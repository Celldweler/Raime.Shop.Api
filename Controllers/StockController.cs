using Microsoft.AspNetCore.Mvc;
using Shop.BL.Stocks;
using Shop.DAL;
using Shop.Dapper.DAL.AppRepositories;
using Shop.Domain.Dto.StockOnHold;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [ApiController]
    [Route("api/stocks")]
    public class StockController : ControllerBase
    {
        private ApplicationDbContext _ctx;
        private DapperStocksRepository _dapperStocksRepository;

        public StockController(ApplicationDbContext ctx,
            DapperStocksRepository dapperStocksRepository)
        {
            _ctx = ctx;
            _dapperStocksRepository = dapperStocksRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_dapperStocksRepository.GetAllStocks());
        }

        [HttpGet("{stockId}")]
        public IActionResult Get(int stockId)
        {
            return Ok(_dapperStocksRepository.GetStockById(stockId));
        }
    }
}
