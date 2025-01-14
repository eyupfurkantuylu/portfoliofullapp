using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioFullApp.Core.Common;
using PortfolioFullApp.Core.DTOs.NavbarItem;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace backendapp.src.PortfolioFullApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NavbarItemController : ControllerBase
    {
        private readonly INavbarItemRepository _navbarItemRepository;
        private readonly ILogger<NavbarItemController> _logger;

        public NavbarItemController(INavbarItemRepository navbarItemRepository, ILogger<NavbarItemController> logger)
        {
            _navbarItemRepository = navbarItemRepository;
            _logger = logger;
        }

        /// <summary>
        /// Tüm navigasyon öğelerini getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<NavbarItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<NavbarItemDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<NavbarItemDto>>>> GetAll()
        {
            try
            {
                var navbarItems = await _navbarItemRepository.GetAllAsync();
                return Ok(ApiResult<IEnumerable<NavbarItemDto>>.SuccessResult(navbarItems, "Navigasyon öğeleri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<NavbarItemDto>>.ErrorResult("Navigasyon öğeleri getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip navigasyon öğesini getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<NavbarItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<NavbarItemDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<NavbarItemDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<NavbarItemDto>>> GetById(string id)
        {
            try
            {
                var navbarItem = await _navbarItemRepository.GetByIdAsync(id);
                if (navbarItem == null)
                {
                    return NotFound(ApiResult<NavbarItemDto>.ErrorResult("Navigasyon öğesi bulunamadı"));
                }
                return Ok(ApiResult<NavbarItemDto>.SuccessResult(navbarItem, "Navigasyon öğesi başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<NavbarItemDto>.ErrorResult("Navigasyon öğesi getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<NavbarItemDto>>> Create([FromBody] CreateNavbarItemDto navbarItem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<NavbarItemDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                _logger.LogInformation($"Gelen veri: Order={navbarItem.Order}, Href={navbarItem.Href}, Icon={navbarItem.Icon}, Label={navbarItem.Label}");

                var createdNavbarItem = await _navbarItemRepository.CreateAsync(navbarItem);

                if (createdNavbarItem == null)
                {
                    return BadRequest(ApiResult<NavbarItemDto>.ErrorResult("Navigasyon öğesi oluşturulamadı"));
                }

                return CreatedAtAction(nameof(GetById),
                    new { id = createdNavbarItem.Id },
                    ApiResult<NavbarItemDto>.SuccessResult(createdNavbarItem, "Navigasyon öğesi başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                var errorMessage = $"Hata detayı: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner exception: {ex.InnerException.Message}";
                }

                _logger.LogError(ex, "Navigasyon öğesi oluşturulurken hata: {ErrorMessage}", errorMessage);

                return BadRequest(ApiResult<NavbarItemDto>.ErrorResult(
                    "Navigasyon öğesi oluşturulurken bir hata oluştu",
                    new List<string> { errorMessage }));
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResult<NavbarItemDto>>> Update([FromBody] UpdateNavbarItemDto navbarItem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<NavbarItemDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var updatedNavbarItem = await _navbarItemRepository.UpdateAsync(navbarItem);
                if (updatedNavbarItem == null)
                {
                    return NotFound(ApiResult<NavbarItemDto>.ErrorResult("Güncellenecek navigasyon öğesi bulunamadı"));
                }

                return Ok(ApiResult<NavbarItemDto>.SuccessResult(updatedNavbarItem, "Navigasyon öğesi başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<NavbarItemDto>.ErrorResult("Navigasyon öğesi güncellenirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> Delete(string id)
        {
            try
            {
                var result = await _navbarItemRepository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Silinecek navigasyon öğesi bulunamadı"));
                }

                return Ok(ApiResult<bool>.SuccessResult(true, "Navigasyon öğesi başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult("Navigasyon öğesi silinirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }
    }
}