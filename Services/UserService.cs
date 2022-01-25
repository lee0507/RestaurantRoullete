using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swilago.Data;
using Swilago.Data.Tables;
using Swilago.Interfaces;
using Swilago.Models;

namespace Swilago.Services
{
    public class UserService : ControllerBase, IUserService
    {
        private readonly IConfiguration _config;
        private readonly BreakContext _context;

        public UserService(IConfiguration config, BreakContext context)
        {
            _config = config;
            _context = context;
        }

        public IActionResult SetAccessUserInfo(UserAccessInfo info)
        {
            if (info == null)
                return BadRequest();

            //lock()
            //{ }

            //try
            //{

            //}
            //catch (Exception e)
            //{
            //    e.Message;
            //}

            // T_Statistics 테이블이 비었으면 1로 초기화
            int statisticsId = 1;

            // 비어있지 않으면 마지막 번호에서 1을 추가한 값으로 초기화
            if (_context.Statistics.Any())
            {
                var checkRow = _context.Statistics.AsNoTracking().OrderByDescending(c => c.StatisticsId).FirstOrDefault();
                statisticsId = checkRow.StatisticsId;
                statisticsId++;
            }

            //write count 


            TStatistics setStatistics = new()
            {
                StatisticsId = statisticsId,
                Email = info.Email,
                RouletteResult = "",
                RouletteList = "",
                ModifiedDate = DateTime.Now
            };

            _context.Statistics.Add(setStatistics);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
