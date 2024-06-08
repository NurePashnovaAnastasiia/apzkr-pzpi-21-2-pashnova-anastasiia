using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Models.Dto;
using LightServeWebAPI.Models;
using LightServeWebAPI.Repositories;
using LightServeWebAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LightServeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;
        private readonly ICafeRepository _cafeRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public TableController(ICafeRepository cafeRepository, ITableRepository tableRepository, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _cafeRepository = cafeRepository;
            _tableRepository = tableRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet("getAllTables")]
        public async Task<ActionResult<List<Table>>> GetAllTables(int cafeId)
        {
            if (cafeId > 0)
            {
                var cafe = await _cafeRepository.GetCafeById(cafeId);

                if (cafe != null)
                {
                    var tables = await _tableRepository.GetAllTables(cafeId);
                    return Ok(tables);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPost("changeTableStatus")]
        public async Task<ActionResult<Table>> ChangeTableStatus(int tableId)
        {
            if (tableId > 0)
            {
                var table = await _tableRepository.GetTableById(tableId);

                if (table != null)
                {
                    table.IsAvailable = !table.IsAvailable;
                    await _tableRepository.UpdateTable(table);
                    return Ok(_stringLocalizer[SharedResourcesKeys.Updated]);
                }
                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }
            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }


        [HttpPost("addTable")]
        public async Task<ActionResult<Table>> AddTable(int cafeId, int size, int number)
        {
            if (cafeId > 0 && size > 0 && number > 0)
            {
                var cafe = await _cafeRepository.GetCafeById(cafeId);

                if (cafe != null)
                {
                    var table = new Table()
                    {
                        Number = number,
                        Size = size,
                        Cafe = cafe,
                        CafeId = cafeId
                    };

                    var addedTable = await _tableRepository.AddTable(table);

                    return Ok(addedTable);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpPost("updateTable")]
        public async Task<ActionResult<Table>> UpdateTable(int tableId, int number)
        {
            if (tableId > 0)
            {
                var table = await _tableRepository.GetTableById(tableId);

                if (table != null)
                {
                    table.Number = number;

                    var updatedTable = await _tableRepository.UpdateTable(table);

                    return Ok(updatedTable);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }

        [HttpDelete("deleteTable")]
        public async Task<ActionResult<Table>> DeleteTable(int tableId)
        {
            if (tableId > 0)
            {
                var table = await _tableRepository.GetTableById(tableId);

                if (table != null)
                {
                    await _tableRepository.DeleteTable(table);
                    return Ok(_stringLocalizer[SharedResourcesKeys.Deleted]);
                }

                return NotFound(_stringLocalizer[SharedResourcesKeys.NotFound]);
            }

            return BadRequest(_stringLocalizer[SharedResourcesKeys.Invalid]);
        }
    }
}
