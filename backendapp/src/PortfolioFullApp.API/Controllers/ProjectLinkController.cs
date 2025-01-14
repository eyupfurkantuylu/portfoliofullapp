using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioFullApp.Core.Common;
using PortfolioFullApp.Core.DTOs.ProjectLink;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;

namespace backendapp.src.PortfolioFullApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectLinkController : ControllerBase
    {
        private readonly IProjectLinkRepository _projectLinkRepository;

        public ProjectLinkController(IProjectLinkRepository projectLinkRepository)
        {
            _projectLinkRepository = projectLinkRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProjectLinkDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProjectLinkDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<ProjectLinkDto>>>> GetAllAsync()
        {
            try
            {
                var projectLinks = await _projectLinkRepository.GetAllAsync();
                return Ok(ApiResult<IEnumerable<ProjectLinkDto>>.SuccessResult(projectLinks, "Proje linkleri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<ProjectLinkDto>>.ErrorResult("Proje linkleri getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ProjectLinkDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<ProjectLinkDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<ProjectLinkDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<ProjectLinkDto>>> GetByIdAsync(string id)
        {
            try
            {
                var projectLink = await _projectLinkRepository.GetByIdAsync(id);
                if (projectLink == null)
                {
                    return NotFound(ApiResult<ProjectLinkDto>.ErrorResult("Proje linki bulunamadı"));
                }
                return Ok(ApiResult<ProjectLinkDto>.SuccessResult(projectLink, "Proje linki başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProjectLinkDto>.ErrorResult("Proje linki getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProjectLinkDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProjectLinkDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<ProjectLinkDto>>>> GetByProjectIdAsync(string projectId)
        {
            try
            {
                var projectLinks = await _projectLinkRepository.GetByProjectIdAsync(projectId);
                return Ok(ApiResult<IEnumerable<ProjectLinkDto>>.SuccessResult(projectLinks, "Proje linkleri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<ProjectLinkDto>>.ErrorResult("Proje linkleri getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<ProjectLinkDto>>> CreateAsync([FromBody] CreateProjectLinkDto projectLink)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<ProjectLinkDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var createdProjectLink = await _projectLinkRepository.CreateAsync(projectLink);
                return CreatedAtAction(nameof(GetByIdAsync),
                    new { id = createdProjectLink.Id },
                    ApiResult<ProjectLinkDto>.SuccessResult(createdProjectLink, "Proje linki başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProjectLinkDto>.ErrorResult("Proje linki oluşturulurken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResult<ProjectLinkDto>>> UpdateAsync([FromBody] UpdateProjectLinkDto projectLink)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<ProjectLinkDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var updatedProjectLink = await _projectLinkRepository.UpdateAsync(projectLink);
                if (updatedProjectLink == null)
                {
                    return NotFound(ApiResult<ProjectLinkDto>.ErrorResult("Güncellenecek proje linki bulunamadı"));
                }

                return Ok(ApiResult<ProjectLinkDto>.SuccessResult(updatedProjectLink, "Proje linki başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProjectLinkDto>.ErrorResult("Proje linki güncellenirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteAsync(string id)
        {
            try
            {
                var result = await _projectLinkRepository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Silinecek proje linki bulunamadı"));
                }

                return Ok(ApiResult<bool>.SuccessResult(true, "Proje linki başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult("Proje linki silinirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }
    }
}