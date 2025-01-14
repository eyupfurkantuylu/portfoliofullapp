using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioFullApp.Core.Common;
using PortfolioFullApp.Core.DTOs.Profile;
using PortfolioFullApp.Core.Interfaces;

namespace backendapp.src.PortfolioFullApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileController(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        /// <summary>
        /// Tüm profil bilgilerini getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProfileDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProfileDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<ProfileDto>>>> GetAll()
        {
            try
            {
                var profiles = await _profileRepository.GetAllAsync();
                return Ok(ApiResult<IEnumerable<ProfileDto>>.SuccessResult(profiles, "Profil bilgileri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<ProfileDto>>.ErrorResult("Profil bilgileri getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip profil bilgisini getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<ProfileDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<ProfileDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<ProfileDto>>> GetById(string id)
        {
            try
            {
                var profile = await _profileRepository.GetByIdAsync(id);
                if (profile == null)
                {
                    return NotFound(ApiResult<ProfileDto>.ErrorResult("Profil bilgisi bulunamadı"));
                }
                return Ok(ApiResult<ProfileDto>.SuccessResult(profile, "Profil bilgisi başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProfileDto>.ErrorResult("Profil bilgisi getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<ProfileDto>>> Create([FromBody] CreateProfileDto createProfileDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<ProfileDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var createdProfile = await _profileRepository.CreateAsync(createProfileDto);
                return CreatedAtAction(nameof(GetById),
                    new { id = createdProfile.Id },
                    ApiResult<ProfileDto>.SuccessResult(createdProfile, "Profil bilgisi başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProfileDto>.ErrorResult("Profil bilgisi oluşturulurken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPut()]
        public async Task<ActionResult<ApiResult<ProfileDto>>> Update([FromBody] UpdateProfileDto profile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<ProfileDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var updatedProfile = await _profileRepository.UpdateAsync(profile);
                if (updatedProfile == null)
                {
                    return NotFound(ApiResult<ProfileDto>.ErrorResult("Güncellenecek profil bilgisi bulunamadı"));
                }

                return Ok(ApiResult<ProfileDto>.SuccessResult(updatedProfile, "Profil bilgisi başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProfileDto>.ErrorResult("Profil bilgisi güncellenirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> Delete(string id)
        {
            try
            {
                var result = await _profileRepository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Silinecek profil bilgisi bulunamadı"));
                }

                return Ok(ApiResult<bool>.SuccessResult(true, "Profil bilgisi başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult("Profil bilgisi silinirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }
    }
}