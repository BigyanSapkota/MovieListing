using Application.DTO.Movie;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Shared.Common;
using Shared.Helper;

namespace Application.Service
{
     public class LanguageService : ILanguageService
    {
        private readonly IGenericRepo<Language,Guid> _languageRepo;
        private readonly ILanguageRepo _languageCustomRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public LanguageService(IGenericRepo<Language, Guid> languageRepo, IUnitOfWork unitOfWork,IHttpContextAccessor contextAccessor, ILanguageRepo languageCustomRepo)
        {
            _languageRepo = languageRepo;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _languageCustomRepo = languageCustomRepo;
        }



        public async Task<ResponseData<LanguageDto>> AddLanguage(CreateLanguageDto dto)
        {
            var user = _contextAccessor.HttpContext?.User;
            var userId = UserInfoHelper.GetUserId(user);
            if(userId == null)
            {
                return new ResponseData<LanguageDto> { Success = false, Message = "User not Authenticated" };
            }

            var allLanguage = await _languageRepo.GetAllAsync();
            if(allLanguage.Any(l => l.LanguageName.ToLower() == dto.LanguageName.ToLower()))
            {
                return new ResponseData<LanguageDto>
                {
                    Success = false,
                    Message = "Language Already Exists"
                };
            }


            var language = new Language
            {
                Id = Guid.NewGuid(),
                LanguageName = dto.LanguageName,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            var response = await _languageRepo.AddAsync(language);
            if(response.Success && response.Data != null)
            {
                await _unitOfWork.CommitAsync();
                var languageDto = new LanguageDto
                {
                    LanguageId = response.Data.Id,
                    LanguageName = response.Data.LanguageName,
                    CreatedBy = response.Data.CreatedBy,
                    CreatedAt = response.Data.CreatedAt
                };
                return new ResponseData<LanguageDto>
                {
                    Success = true,
                    Message = "Language added successfully",
                    Data = languageDto
                };
            }
            return new ResponseData<LanguageDto>
            {
                Success = false,
                Message = "Failed to add language",
                Data = null
            };
        }





        public async Task<ResponseData<bool>> DeleteLanguage(Guid languageId)
        {
            var user = _contextAccessor.HttpContext?. User;
            var loginUser = UserInfoHelper.GetUserId(user);
            var response = await _languageRepo.GetByIdAsync(languageId);
            if(response.Success && response.Data != null)
            {
                response.Data.DeletedBy = loginUser;
                response.Data.DeletedAt = DateTime.Now;
                var result = await _languageRepo.DeleteAsync(response.Data);
                    if(result.Success)
                {
                    await _unitOfWork.CommitAsync();
                    return new ResponseData<bool>
                    {
                        Success = true,
                        Message = "Data deleted Successfully"
                    };
                }
            }
            return new ResponseData<bool>
            {
                Success = false,
                Message = "Failed to delete data"
            };
        }




        public async Task<ResponseData<PageResult<LanguageDto>>> GetAllLanguages(int pageNumber, int pageSize)
        {
            var response = await _languageCustomRepo.GetAllLanguage(pageNumber,pageSize);
            if(response.Success && response.Data != null)
            {
                var language = new PageResult<LanguageDto>
                {
                    Items = response.Data.Items.Select(l => new LanguageDto
                    {
                        LanguageId = l.Id,
                        LanguageName = l.LanguageName,
                        CreatedBy = l.CreatedBy,
                        CreatedAt = l.CreatedAt,
                        UpdatedBy = l.UpdatedBy,
                        UpdatedAt = l.UpdatedAt,
                        DeletedBy = l.DeletedBy,
                        DeletedAt = l.DeletedAt
                    }).ToList(),
                    TotalItems = response.Data.TotalItems,
                    PageNumber = response.Data.PageNumber,
                    PageSize = response.Data.PageSize
                };
                return new ResponseData<PageResult<LanguageDto>>
                {
                    Success = true,
                    Message = language.Items.Any() ? "Languages retrieved successfully" : "No languages found",
                    Data = language
                };

            }
            return new ResponseData<PageResult<LanguageDto>>
            {
                Success = false,
                Message = "No languages found"
            };

        }




        public async Task<ResponseData<LanguageDto>> GetLanguageById(Guid languageId)
        {
            var response = await _languageRepo.GetByIdAsync(languageId);
            if (response.Success && response.Data != null)
            {
                var language = new LanguageDto
                {
                    LanguageId = response.Data.Id,
                    LanguageName = response.Data.LanguageName,
                    CreatedBy = response.Data.CreatedBy,
                    CreatedAt = response.Data.CreatedAt,
                    UpdatedBy = response.Data.UpdatedBy,
                    UpdatedAt = response.Data.UpdatedAt,
                    DeletedBy = response.Data.DeletedBy,
                    DeletedAt = response.Data.DeletedAt
                };
                return new ResponseData<LanguageDto>
                    {
                    Success = true,
                    Message = "Language retrieved successfully",
                    Data = language
                };
            }
            return new ResponseData<LanguageDto>
            {
                Success = false,
                Message = "Language not found",
                Data = null
            };
        }


        public async Task<ResponseData<LanguageDto>> UpdateLanguage(UpdateLanguageDto updateLanguageDto)
        {
            var user = _contextAccessor.HttpContext?.User;
            var loginUser = UserInfoHelper.GetUserId(user);
            var response = await _languageRepo.GetByIdAsync(updateLanguageDto.LanguageId);
            if (response.Success && response.Data != null)
            {
                response.Data.LanguageName = updateLanguageDto.LanguageName;
                response.Data.UpdatedBy = loginUser;
                response.Data.UpdatedAt = DateTime.Now;
                var result = await _languageRepo.UpdateAsync(response.Data);
                if (result.Success && result.Data != null)
                {
                    await _unitOfWork.CommitAsync();
                    var languageDto = new LanguageDto
                    {
                        LanguageId = result.Data.Id,
                        LanguageName = result.Data.LanguageName,
                        CreatedBy = result.Data.CreatedBy,
                        CreatedAt = result.Data.CreatedAt,
                        UpdatedBy = result.Data.UpdatedBy,
                        UpdatedAt = result.Data.UpdatedAt,
                        DeletedBy = result.Data.DeletedBy,
                        DeletedAt = result.Data.DeletedAt
                    };
                    return new ResponseData<LanguageDto>
                    {
                        Success = true,
                        Message = "Language updated successfully",
                        Data = languageDto
                    };
                }
            }
            return new ResponseData<LanguageDto>
            {
                Success = false,
                Message = "Unable to Update"
            };
        }


        public async Task<ResponseData<int>> GetActiveLanguageCount()
        {
            var activeLanguageCount = await _languageRepo.GetCount(l => l.IsActive);
            return new ResponseData<int>
            {
                Success = true,
                Message = "Active language count retrieved successfully",
                Data = activeLanguageCount
            };
        }




    }
}
