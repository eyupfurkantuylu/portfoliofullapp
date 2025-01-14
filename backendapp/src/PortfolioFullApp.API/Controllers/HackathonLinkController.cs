using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioFullApp.Core.Common;
using PortfolioFullApp.Core.DTOs.HackathonLink;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;

namespace backendapp.src.PortfolioFullApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HackathonLinkController : ControllerBase
    {
        private readonly IHackathonLinkRepository _hackathonLinkRepository;

        public HackathonLinkController(IHackathonLinkRepository hackathonLinkRepository)
        {
            _hackathonLinkRepository = hackathonLinkRepository;
        }

        /// <summary>
        /// Belirtilen Hackathon ID'sine ait tüm linkleri getirir
        /// </summary>
        [HttpGet("hackathon/{hackathonId}")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<HackathonLinkDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<HackathonLinkDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<HackathonLinkDto>>>> GetAllByHackathonId(string hackathonId)
        {
            try
            {
                var links = await _hackathonLinkRepository.GetAllByHackathonIdAsync(hackathonId);
                return Ok(ApiResult<IEnumerable<HackathonLinkDto>>.SuccessResult(links, "Hackathon linkleri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<HackathonLinkDto>>.ErrorResult(
                    "Hackathon linkleri getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip linki getirir
        /// </summary>
        [HttpGet("{hackathonLinkId}")]
        [ProducesResponseType(typeof(ApiResult<HackathonLinkDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<HackathonLinkDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResult<HackathonLinkDto>>> GetById(string hackathonLinkId)
        {
            try
            {
                var link = await _hackathonLinkRepository.GetByIdAsync(hackathonLinkId);
                if (link == null)
                {
                    return NotFound(ApiResult<HackathonLinkDto>.ErrorResult("Link bulunamadı"));
                }
                return Ok(ApiResult<HackathonLinkDto>.SuccessResult(link, "Link başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<HackathonLinkDto>.ErrorResult(
                    "Link getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen Hackathon için birden fazla link oluşturur
        /// </summary>
        [HttpPost("hackathon/{hackathonId}")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<HackathonLinkDto>>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResult<IEnumerable<HackathonLinkDto>>>> CreateMany(string hackathonId, [FromBody] IEnumerable<CreateHackathonLinkDto> links)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<IEnumerable<HackathonLinkDto>>.ErrorResult(
                        "Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var createdLinks = await _hackathonLinkRepository.CreateManyAsync(hackathonId, links);
                return CreatedAtAction(
                    nameof(GetAllByHackathonId),
                    new { hackathonId },
                    ApiResult<IEnumerable<HackathonLinkDto>>.SuccessResult(createdLinks, "Linkler başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<HackathonLinkDto>>.ErrorResult(
                    "Linkler oluşturulurken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip linki günceller
        /// </summary>
        [HttpPut("{hackathonLinkId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<bool>>> Update(string hackathonLinkId, [FromBody] UpdateHackathonLinkDto link)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<bool>.ErrorResult(
                        "Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }
                var result = await _hackathonLinkRepository.UpdateAsync(hackathonLinkId, link);
                return Ok(ApiResult<bool>.SuccessResult(result, "Link başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult(
                    "Link güncellenirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen Hackathon'a ait tüm linkleri siler
        /// </summary>
        [HttpDelete("hackathon/{hackathonId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResult<bool>>> DeleteByHackathonId(string hackathonId)
        {
            try
            {
                var result = await _hackathonLinkRepository.DeleteByHackathonIdAsync(hackathonId);
                if (!result)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Silinecek link bulunamadı"));
                }
                return Ok(ApiResult<bool>.SuccessResult(true, "Linkler başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult(
                    "Linkler silinirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{hackathonLinkId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResult<bool>>> DeleteById(string hackathonLinkId)
        {
            var result = await _hackathonLinkRepository.DeleteByIdAsync(hackathonLinkId);
            return Ok(ApiResult<bool>.SuccessResult(result, "Link başarıyla silindi"));
        }
    }
}