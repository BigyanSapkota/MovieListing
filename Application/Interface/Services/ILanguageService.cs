using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Org.BouncyCastle.Asn1.Ocsp;
using Shared.Common;

namespace Application.Interface.Services
{
     public interface ILanguageService
    {
        public Task<ResponseData<LanguageDto>> AddLanguage(CreateLanguageDto dto);
        public Task<ResponseData<bool>> DeleteLanguage(Guid languageId);
        public Task<ResponseData<LanguageDto>> UpdateLanguage(UpdateLanguageDto updateLanguageDto);
        public Task<ResponseData<LanguageDto>> GetLanguageById(Guid languageId);
        public Task<ResponseData<PageResult<LanguageDto>>> GetAllLanguages(int pageNumber, int pageSize);
        public Task<ResponseData<int>> GetActiveLanguageCount();

    }
}
