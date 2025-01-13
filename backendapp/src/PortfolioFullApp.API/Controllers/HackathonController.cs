using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioFullApp.Core.Common;
using PortfolioFullApp.Core.DTOs.Hackathon;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;

namespace backendapp.src.PortfolioFullApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HackathonController : ControllerBase
    {
        private readonly IHackathonRepository _hackathonRepository;

        public HackathonController(IHackathonRepository hackathonRepository)
        {
            _hackathonRepository = hackathonRepository;
        }

        /// <summary>
        /// Tüm hackathon bilgilerini getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<HackathonDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<HackathonDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<HackathonDto>>>> GetAll()
        {
            try
            {
                var hackathons = await _hackathonRepository.GetAllAsync();
                return Ok(ApiResult<IEnumerable<HackathonDto>>.SuccessResult(hackathons, "Hackathon bilgileri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<HackathonDto>>.ErrorResult("Hackathon bilgileri getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip hackathon bilgisini getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<HackathonDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<HackathonDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<HackathonDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<HackathonDto>>> GetById(string id)
        {
            try
            {
                var hackathon = await _hackathonRepository.GetByIdAsync(id);
                if (hackathon == null)
                {
                    return NotFound(ApiResult<HackathonDto>.ErrorResult("Hackathon bilgisi bulunamadı"));
                }
                return Ok(ApiResult<HackathonDto>.SuccessResult(hackathon, "Hackathon bilgisi başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<HackathonDto>.ErrorResult("Hackathon bilgisi getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<HackathonDto>>> Create([FromBody] CreateHackathonDto hackathon)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<HackathonDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var createdHackathon = await _hackathonRepository.CreateAsync(hackathon);
                return CreatedAtAction(nameof(GetById),
                    new { id = createdHackathon.Id },
                    ApiResult<HackathonDto>.SuccessResult(createdHackathon, "Hackathon bilgisi başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<HackathonDto>.ErrorResult("Hackathon bilgisi oluşturulurken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResult<HackathonDto>>> Update([FromBody] UpdateHackathonDto hackathon)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<HackathonDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var updatedHackathon = await _hackathonRepository.UpdateAsync(hackathon);
                if (updatedHackathon == null)
                {
                    return NotFound(ApiResult<HackathonDto>.ErrorResult("Güncellenecek hackathon bilgisi bulunamadı"));
                }

                return Ok(ApiResult<HackathonDto>.SuccessResult(updatedHackathon, "Hackathon bilgisi başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<HackathonDto>.ErrorResult("Hackathon bilgisi güncellenirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> Delete(string id)
        {
            try
            {
                var result = await _hackathonRepository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Silinecek hackathon bilgisi bulunamadı"));
                }

                return Ok(ApiResult<bool>.SuccessResult(true, "Hackathon bilgisi başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult("Hackathon bilgisi silinirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }
    }
}