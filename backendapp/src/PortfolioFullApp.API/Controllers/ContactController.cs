using Microsoft.AspNetCore.Mvc;
using PortfolioFullApp.Core.Common;
using PortfolioFullApp.Core.DTOs.Contact;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;

namespace PortfolioFullApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;

        /// <summary>
        /// Contact Controller Constructor
        /// </summary>
        public ContactController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        /// <summary>
        /// Tüm iletişim bilgilerini getirir
        /// </summary>
        /// <returns>İletişim bilgileri listesi</returns>
        /// <response code="200">İletişim bilgileri başarıyla getirildi</response>
        /// <response code="400">İstek sırasında bir hata oluştu</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ContactDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ContactDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<ContactDto>>>> GetAll()
        {
            try
            {
                var contacts = await _contactRepository.GetAllAsync();
                return Ok(ApiResult<IEnumerable<ContactDto>>.SuccessResult(contacts, "İletişim bilgileri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<ContactDto>>.ErrorResult("İletişim bilgileri getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip iletişim bilgisini getirir
        /// </summary>
        /// <param name="id">İletişim bilgisi ID'si</param>
        /// <returns>İletişim bilgisi</returns>
        /// <response code="200">İletişim bilgisi başarıyla getirildi</response>
        /// <response code="404">İletişim bilgisi bulunamadı</response>
        /// <response code="400">İstek sırasında bir hata oluştu</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ContactDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<ContactDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<ContactDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<ContactDto>>> GetById(string id)
        {
            try
            {
                var contact = await _contactRepository.GetByIdAsync(id);
                if (contact == null)
                {
                    return NotFound(ApiResult<ContactDto>.ErrorResult("İletişim bilgisi bulunamadı"));
                }
                return Ok(ApiResult<ContactDto>.SuccessResult(contact, "İletişim bilgisi başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ContactDto>.ErrorResult("İletişim bilgisi getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<ContactDto>>> Create([FromBody] Contact contact)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<ContactDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                contact.Id = Guid.NewGuid().ToString();
                var createdContact = await _contactRepository.CreateAsync(contact);
                return CreatedAtAction(nameof(GetById),
                    new { id = createdContact.Id },
                    ApiResult<ContactDto>.SuccessResult(createdContact, "İletişim bilgisi başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ContactDto>.ErrorResult("İletişim bilgisi oluşturulurken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<ContactDto>>> Update(string id, [FromBody] Contact contact)
        {
            try
            {
                if (id != contact.Id)
                {
                    return BadRequest(ApiResult<ContactDto>.ErrorResult("ID'ler eşleşmiyor"));
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<ContactDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var updatedContact = await _contactRepository.UpdateAsync(contact);
                if (updatedContact == null)
                {
                    return NotFound(ApiResult<ContactDto>.ErrorResult("Güncellenecek iletişim bilgisi bulunamadı"));
                }

                return Ok(ApiResult<ContactDto>.SuccessResult(updatedContact, "İletişim bilgisi başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<ContactDto>.ErrorResult("İletişim bilgisi güncellenirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> Delete(string id)
        {
            try
            {
                var result = await _contactRepository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Silinecek iletişim bilgisi bulunamadı"));
                }

                return Ok(ApiResult<bool>.SuccessResult(true, "İletişim bilgisi başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult("İletişim bilgisi silinirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }
    }
}