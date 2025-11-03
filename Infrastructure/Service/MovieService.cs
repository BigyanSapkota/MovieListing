using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Shared.Common;
using Shared.Helper;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Application.Service
{
     public class MovieService : IMovieService
    {
     
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IGenericRepo<Movie,Guid> _movieRepoo;
        private readonly IGenericRepo<Genre, Guid> _genreRepo;
        private readonly IGenericRepo<Language, Guid> _languageRepo;
        private readonly IMovieRepo _movieRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UploadToDrive _uploadToDrive;
        private readonly EmailHelper _emailHelper;
        private readonly IMovieNotificationJob _notificationJob;
        private readonly IPushNotificationService _pushNotification;
        private readonly IDeviceTokenRepo _deviceTokenRepo;
        private readonly IDeleteRequestService _deleteRequestService;
        public MovieService(IMovieRepo movieRepo, IMapper mapper, IGenericRepo<Movie,Guid> movieRepoo, IUnitOfWork unitOfWork,
                            IGenericRepo<Genre, Guid> genreRepo, IGenericRepo<Language, Guid> languageRepo, IConfiguration configuration,
                           UploadToDrive uploadToDrive, EmailHelper emailHelper,IMovieNotificationJob notificationJob,IPushNotificationService pushNotification,
                           IDeviceTokenRepo deviceTokenRepo, IDeleteRequestService deleteRequestService)
        {
            _mapper = mapper;
            _movieRepoo = movieRepoo;
            _movieRepo = movieRepo;
            _unitOfWork = unitOfWork;
            _genreRepo = genreRepo;
            _languageRepo = languageRepo;
            _configuration = configuration;
            _uploadToDrive = uploadToDrive;
            _emailHelper = emailHelper;
            _notificationJob = notificationJob;
            _pushNotification = pushNotification;
            _deviceTokenRepo = deviceTokenRepo;
            _deleteRequestService = deleteRequestService;
        }




        public async Task<ResponseData<MovieDto>> AddMovieAsync(CreateMovieDto movieDto, string loginuser)
        {
            var movie = _mapper.Map<Movie>(movieDto);

            movie.Id = Guid.NewGuid();
            movie.CreatedBy = loginuser;
            movie.CreatedAt = DateTime.Now;
            if (loginuser == null)
                throw new UnauthorizedAccessException("You cannot add movie");
            if (movieDto == null)
                throw new ArgumentException("Movie data must be added");

            ////////---------Uploading Poster to Google Drive-------------------////////

            if (movieDto.PosterUrl != null && movieDto.PosterUrl.Length > 0)
            {

                string folderId = _configuration["GoogleDrive:FolderId"];



                string mimeType = movieDto.PosterUrl.ContentType;
               

                using var stream = movieDto.PosterUrl.OpenReadStream();
                string fileId = _uploadToDrive.UploadFile(
                    stream,
                    movieDto.PosterUrl.FileName,
                    mimeType,
                    folderId
                    
                );

                movie.PosterUrl = $"https://drive.google.com/uc?id={fileId}";
            }

                movie.YouTubeLink = movieDto.YouTubeLink;



                movie.MovieGenre = new List<MovieGenre>();
                movie.MovieLanguage = new List<MovieLanguage>();

                // Get existing genres from database
                var existingGenre = await _movieRepo.GetByGenreNameAsync(movieDto.Genre);

                if(existingGenre == null)
                {
                    return new ResponseData<MovieDto>
                    {
                        Success = false,
                        Message = "No genres found",
                        Data = null
                    };
                }


            //// Add new genres if they do not exist
            //foreach (var genre in movieDto.Genre.Except(existingGenre.Select(g => g.GenreName)))
            //{
            //    var newGenre = new Genre
            //    {
            //        Id = Guid.NewGuid(),
            //        GenreName = genre
            //    };
            //    await _genreRepo.AddAsync(newGenre);
            //    existingGenre.Add(newGenre);
            //}

            // Add data in bridge table movie_genre
            foreach (var genre in existingGenre)
                {
                    movie.MovieGenre.Add(new MovieGenre
                    {
                        GenreId = genre.Id,
                        MovieId = movie.Id
                    });
                }


                // Get existing languages from database
            var existingLanguage = await _movieRepo.GetByLanguageNameAsync(movieDto.Language);
            if (existingLanguage == null)
            {
                return new ResponseData<MovieDto>
                {
                    Success = false,
                    Message = "No Language found",
                    Data = null
                };
            }


            //// Add new languages if they do not exist
            //foreach (var language in movieDto.Language.Except(existingLanguage.Select(l => l.LanguageName)))
            //{
            //    var newLanguage = new Language
            //    {
            //        Id = Guid.NewGuid(),
            //        LanguageName = language
            //    };
            //    await _languageRepo.AddAsync(newLanguage);
            //    existingLanguage.Add(newLanguage);
            //}

            // Add data in bridge table movie_language
            foreach (var language in existingLanguage)
                {
                    movie.MovieLanguage.Add(new MovieLanguage()
                    {
                        LanguageId = language.Id,
                        MovieId = movie.Id
                    });
                }



                var result = await _movieRepoo.AddAsync(movie);

                if (result.Success == true)
                {
                
                    await _unitOfWork.CommitAsync();
                _notificationJob.ScheduleMovieNotifications(result.Data);
                //var response = await _deviceTokenRepo.GetAllToken();
                var tokens = (await _deviceTokenRepo.GetAllToken()).Select(t => t.Token).ToList();

                    var body = $"New Movie : {result.Data.Title}  will release on {result.Data.ReleaseDate} ";
                //await _pushNotification.SendNotificationAsync(res.Token,result.Data.Title , body);
                await _pushNotification.SendNotificationsToMultipleUsersAsync(tokens, result.Data.Title , body);



                var movieData = _mapper.Map<MovieDto>(result.Data);
                    return new ResponseData<MovieDto>
                    {
                        Success = true,
                        Message = "Movie added successfully",
                        Data = movieData
                    };
                }
                return new ResponseData<MovieDto>
                {
                    Success = false,
                    Message = "Failed to add movie",
                    Data = null
                };

        }
        




        public async Task<ResponseData<MovieDto>> DeleteMovieAsync(Guid id,string loginuser)
        {
            var existRequest = await _deleteRequestService.GetRequestByMovie(id);
            if( existRequest != null)
            {
                throw new ArgumentException("Movie Delete Request Already Exists !");
            }
            var result = await _deleteRequestService.CreateDeleteRequestAsync(id, loginuser);

            //var data = await _movieRepoo.GetByIdAsync(id);

            //if (data == null)
            //    throw new KeyNotFoundException("Movie not found");
            //data.Data.DeletedBy = loginuser;
            //data.Data.DeletedAt = DateTime.Now;

            //if (data.Data == null || data.Data.CreatedBy != loginuser)
            //{
            //    throw new UnauthorizedAccessException("You are not authorize to delete");
            //}

            //var result = await _movieRepoo.DeleteAsync(data.Data);

            if (result != null)
            {
                await _unitOfWork.CommitAsync();
                return new ResponseData<MovieDto>
                {
                    Success = true,
                    Message = "Delete Request Created Successfully",
                };
            }

            return new ResponseData<MovieDto> { Success = false, Message = "Delete Request Cannot be Created " };

        }





        public async Task<ResponseData<PageResult<MovieShowDto>>> GetAllMoviesAsync(int pageNumber,int pageSize)
        {
            var pagedResult = await _movieRepo.GetAllMovies(pageNumber, pageSize);
            if (pagedResult.Data.Items.Count == 0)
            {
                throw new KeyNotFoundException("Movie Not Found");
            }

            var pagedResultDto = new PageResult<MovieShowDto>
            {
                Items = pagedResult.Data.Items,
                TotalItems = pagedResult.Data.TotalItems,
                PageNumber = pagedResult.Data.PageNumber,
                PageSize = pagedResult.Data.PageSize
            };

            if (pagedResult != null)
            {
                return new ResponseData<PageResult<MovieShowDto>>
                {
                    Success = true,
                    Message = "Movies retrieved successfully",
                    Data = pagedResultDto
                };
            }
            return new ResponseData<PageResult<MovieShowDto>>
            {
                Success = false,
                Message = "No movies found",
                Data = null
            };
          
        }

       


        public async Task<ResponseData<MovieDto>> GetMovieByIdAsync(Guid id)
        {
            var result = await _movieRepo.GetById(id);

            if (result.Success && result.Data != null)
            {
                var movieData = _mapper.Map<MovieDto>(result.Data);

                return new ResponseData<MovieDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data = movieData
                };
            }


            return new ResponseData<MovieDto>
            {
                Success = false,
                Message = result.Message ?? "Movie not found",
                Data = null
            };
            //throw new NotImplementedException();
        }

       



        public async Task<ResponseData<MovieDto>> UpdateMovieAsync([FromForm]MovieDto movieDto,string loginuser)
        {
       
                var response = await _movieRepoo.GetByIdAsync(movieDto.Id);


            if (response == null || response.Data == null)
            {
                return new ResponseData<MovieDto>
                {
                    Success = false,
                    Message = "Movie Doesnot Exist !",
                    Data = null
                };
            }
            else
            {
                if (response.Data.CreatedBy != loginuser)
                {
                    return new ResponseData<MovieDto>
                    {
                        Success = false,
                        Message = "You Cannot Update This Movie",
                        Data = null
                    };
                }
            }

            response.Data.UpdatedBy = loginuser;
            response.Data.UpdatedAt = DateTime.Now;


            var existingMovie = response.Data;
            
            existingMovie.Title = movieDto.Title;
            existingMovie.Description = movieDto.Description;
            //existingMovie.Genre = movieDto.Genre;
            existingMovie.ReleaseDate = movieDto.ReleaseDate;
            //existingMovie.Language = movieDto.Language;
            existingMovie.Duration = movieDto.Duration;
            //existingMovie.PosterUrl = movieDto.PosterUrl;
            existingMovie.YouTubeLink = movieDto.YouTubeLink;


            if (movieDto.PosterUrl != null && movieDto.PosterUrl.Length > 0)
            {

                string folderId = _configuration["GoogleDrive:FolderId"];



                string mimeType = movieDto.PosterUrl.ContentType;
                
                using var stream = movieDto.PosterUrl.OpenReadStream();
                string fileId = _uploadToDrive.UploadFile(
                    stream,
                    movieDto.PosterUrl.FileName,
                    mimeType,
                    folderId
               
                );

                existingMovie.PosterUrl = $"https://drive.google.com/uc?id={fileId}";
            }



            var result = await _movieRepoo.UpdateAsync(existingMovie);
            if (!result.Success || result.Data == null)
            {
                return new ResponseData<MovieDto>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to update movie",
                    Data = null
                };
            }

            await _unitOfWork.CommitAsync();

            var updatedMovieData = _mapper.Map<MovieDto>(result.Data);
            return new ResponseData<MovieDto>
            {
                Success = result.Success,
                Message = result.Message,
                Data = updatedMovieData
            };

        }





        public async Task<ResponseData<List<MovieShowDto>>> GetMovieByFilterAsync([FromForm]MovieFilterDto filterDto)
        {
            var response = await _movieRepo.GetMovieByFilterAsync(filterDto);
            if (response == null || response.Data == null)
            {
                return new ResponseData<List<MovieShowDto>>
                {
                    Success = false,
                    Message = "No movies found matching the filter criteria",
                    Data = null
                };
            }
            

            //var movieData = _mapper.Map<List<MovieDto>>(response.Data);
            return new ResponseData<List<MovieShowDto>>
            {
                Success = true,
                Message = "Movies retrieved successfully",
                Data = response.Data
            };
        }


        public async Task<ResponseData> ShareMovieAsync(Guid movieId, string email,string loginUser, string userEmail)
        {
            var response = await _movieRepo.GetById(movieId);

            if (response == null || response.Data == null)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Movie not found"
                };
            }





            var genre = response.Data.Genre != null && response.Data.Genre.Any() ? string.Join(", ", response.Data.Genre) : "N/A";
            var language = response.Data.Language != null && response.Data.Language.Any() ? string.Join(",", response.Data.Language) : "N/A";




            var subject = $"Check out this movie : {response.Data.Title}";
            var body = $"Hello,</b>{email},\n\n I want to share a movie with you.\n\n" + $"Movie Title:</b>{response.Data.Title}\n\n"+
                $"Description:</b>{response.Data.Description}\n\n" + $"Release Date:</b>{response.Data.ReleaseDate}\n\n" +
                 $"Genre :</b>{genre}\n\n" + $"Language :</b>{language}\n\n" +
                $"Duration:</b>{response.Data.Duration}\n\n" + $"Poster URL:</b>{response.Data.PosterLink}\n\n" +
                $"YouTube Link:</b>{response.Data.YouTubeLink}\n\n" +$"Thank you!";


            _emailHelper.SendEmail(
                loginUser,
                userEmail,
                email,
                email,
                subject,
                body
                );

            return new ResponseData
            {
                Success = true,
                Message = "Movie shared successfully via email"

            };

        }

 
        public async Task<ResponseData<int>> GetAllMovieCount()
        {
            var count = await _movieRepoo.GetCount();
            if(count >= 0)
            {
                return new ResponseData<int>
                {
                    Success = true,
                    Message = "Total Movie Count",
                    Data = count
                };
            }
            return new ResponseData<int>
            {
                Success = false,
                Message = "Failed to retrieve movie count",
            };
        }



        public async Task<ResponseData<int>> GetActiveMovieCount()
        {
            var response = await _movieRepoo.GetCount(x => x.IsActive);
            if (response > 0)
            {
                return new ResponseData<int>
                {
                    Success = true,
                    Message = "Active Movie Count",
                    Data = response
                };
            }
            else
            {
                return new ResponseData<int>
                {
                    Success = false,
                    Message = "No Active Movies",
                };
            }
        }






    }
}
