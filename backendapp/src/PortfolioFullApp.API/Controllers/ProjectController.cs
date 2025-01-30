using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioFullApp.Core.Common;
using PortfolioFullApp.Core.DTOs.Project;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;

namespace backendapp.src.PortfolioFullApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// Tüm projeleri getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProjectDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ProjectDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<ProjectDto>>>> GetAllAsync()
        {
            try
            {
                var projects = await _projectRepository.GetAllAsync();
                return Ok(ApiResult<IEnumerable<ProjectDto>>.SuccessResult(projects, "Projeler başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<ProjectDto>>.ErrorResult("Projeler getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip projeyi getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ProjectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<ProjectDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<ProjectDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<ProjectDto>>> GetByIdAsync(string id)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    return NotFound(ApiResult<ProjectDto>.ErrorResult("Proje bulunamadı"));
                }
                return Ok(ApiResult<ProjectDto>.SuccessResult(project, "Proje başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProjectDto>.ErrorResult("Proje getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<ProjectDto>>> CreateAsync([FromBody] CreateProjectDto project)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<ProjectDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var createdProject = await _projectRepository.CreateAsync(project);
                return CreatedAtAction(nameof(GetByIdAsync),
                    new { id = createdProject.Id },
                    ApiResult<ProjectDto>.SuccessResult(createdProject, "Proje başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProjectDto>.ErrorResult("Proje oluşturulurken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResult<ProjectDto>>> UpdateAsync([FromBody] UpdateProjectDto project)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<ProjectDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var updatedProject = await _projectRepository.UpdateAsync(project);
                if (updatedProject == null)
                {
                    return NotFound(ApiResult<ProjectDto>.ErrorResult("Güncellenecek proje bulunamadı"));
                }

                return Ok(ApiResult<ProjectDto>.SuccessResult(updatedProject, "Proje başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ProjectDto>.ErrorResult("Proje güncellenirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteAsync(string id)
        {
            try
            {
                var result = await _projectRepository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Silinecek proje bulunamadı"));
                }

                return Ok(ApiResult<bool>.SuccessResult(true, "Proje başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult("Proje silinirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }
    }
}