using Borago.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swilago.Data;
using Swilago.Data.Tables;
using Swilago.Interfaces;
using Swilago.Models;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace Swilago.Services
{
    public class RestaurantService : ControllerBase, IRestaurantService
    {
        private readonly IConfiguration _config;
        private readonly BreakContext _context;

        public RestaurantService(IConfiguration config, BreakContext context)
        {
            _config = config;
            _context = context;
        }

        // 식당 전체 조회
        public IActionResult GetRestaurantList()
        {
            // Statistics 테이블에서 가장 마지막 StatisticsId에 있는 Email 가져오기. 가져온 Email로 접속자 확인 가능
            var email = _context.Statistics.AsNoTracking().OrderByDescending(i => i.Email).FirstOrDefault();

            string rouletteList = "";
            int restaurantNamesCount = 0;
            if (email == null)
            {
                // 첫 접속자면 저장된 Email이 없기에 기본 T_Restaurant로 업데이트.
                rouletteList = "솔낭구,진씨화로,봉된장,중국집,소머리국밥,";
                restaurantNamesCount = 5;
            }
            else
            {
                // 조건 추가하여 검색 => 동일한 Email이면서 RouletteList가 Empty("")가 아닌 가장 마지막 StatisticsId의 RouletteList 가져오기.
                // '솔낭구,진씨화로,봉된장,중국집,소머리국밥,'
                rouletteList = _context.Statistics.AsNoTracking().Where(i => i.Email.Equals(email) && i.RouletteList != "").OrderByDescending(i => i.RouletteList).FirstOrDefault().ToString();

                // 가져온 RouletteList는 String이기에 Split해서 RestaurantList 클래스 최신화.
                string[] restaurantNames = rouletteList.ToString().Split(',');
                restaurantNamesCount = restaurantNames.Count() - 1;
            }

            // RestaurantList클래스를 Procedure에 넘겨서 T_Restaurant 최신화 업데이트.
            using (SqlConnection dbConnection = new(_config["SqlServerConnection"]))
            {
                dbConnection.Open();
                using (SqlCommand command = dbConnection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = "EXEC dbo.P_SetRouletteList @RouletteResultListCount, @RestaurantList";

                    // rouletteResultListCount 값을 프로시져로 전달 코드
                    SqlParameter paramRouletteResultListCount = new("@RouletteResultListCount", restaurantNamesCount);
                    SqlParameter paramRestaurantListString = new("@RestaurantList", rouletteList);

                    command.Parameters.Add(paramRouletteResultListCount);
                    command.Parameters.Add(paramRestaurantListString);

                    command.ExecuteNonQuery();
                }
            }

            return NoContent();
        }

        // 식당 하나 조회
        public Payload GetRestaurant(int restaurantId)
        {
            Payload payload = new();

            try
            {
                var vaildateRow = _context.Restaurant.AsNoTracking().Where(c => c.RestaurantId == restaurantId).SingleOrDefault();

                if (vaildateRow != null)
                {
                    var query = from restaurant in _context.Restaurant.AsNoTracking()
                                where restaurant.RestaurantId == restaurantId
                                select new
                                {
                                    id = restaurant.RestaurantId,
                                    text = restaurant.RestaurantName
                                };

                    var result = query.ToList();
                    payload.Message = JsonConvert.SerializeObject(result, Formatting.Indented);
                }
                
            }
            catch (Exception ex)
            {
                SetErrorMessages(ref payload, ex);
            }

            return payload;
        }

        // 룰렛 당첨결과 저장
        public IActionResult AddRouletteResult(string rouletteResult)
        {
            if (string.IsNullOrEmpty(rouletteResult))
                return BadRequest();

            var getRow = _context.Statistics.AsNoTracking().OrderByDescending(i => i.StatisticsId).FirstOrDefault();
            
            if (getRow != null)
            {
                getRow.RouletteResult = rouletteResult;
                getRow.ModifiedDate = DateTime.Now;

                _context.Statistics.Update(getRow);
                _context.SaveChanges();
            }

            return NoContent();
        }

        // 룰렛 리스트 최신화
        public IActionResult AddRouletteList(Roulettes jsonRouletteResultList) // List: { id, text, fillStyle, textFillStyle }
        {
            if (jsonRouletteResultList.RouletteList == null)
                return BadRequest();

            int rouletteResultListCount = jsonRouletteResultList.RouletteList.Count;

            // rouletteResultList에서 text만 따로 뽑기
            string restaurantList = "";
            foreach (var jsonRouletteResult in jsonRouletteResultList.RouletteList)
            {
                //string jsonString = System.Text.Json.JsonSerializer.Serialize(jsonRouletteResult); // { "id", "text", "fillStyle", "textFillStyle" }
                //var jsonKey = JsonConvert.DeserializeObject<RouletteList>(jsonRouletteResult); // insert jsonString to Models.RouletteResultList.cs
                restaurantList += (jsonRouletteResult.Text + @",");
            }

            // T_Statistics에 RouletteList 업데이트
            var getRow = _context.Statistics.AsNoTracking().OrderByDescending(i => i.StatisticsId).FirstOrDefault();

            if (getRow != null)
            {
                getRow.RouletteList = restaurantList;

                _context.Statistics.Update(getRow);
                _context.SaveChanges();
            }

            // T_Restaurant 변경
            using (SqlConnection dbConnection = new(_config["SqlServerConnection"]))
            {
                dbConnection.Open();
                using (SqlCommand command = dbConnection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = "EXEC dbo.P_SetRouletteList @RouletteResultListCount, @RestaurantList";

                    // rouletteResultListCount 값을 프로시져로 전달 코드
                    SqlParameter paramRouletteResultListCount = new("@RouletteResultListCount", rouletteResultListCount);
                    SqlParameter paramRestaurantListString = new("@RestaurantList", restaurantList);
                    
                    command.Parameters.Add(paramRouletteResultListCount);
                    command.Parameters.Add(paramRestaurantListString);

                    command.ExecuteNonQuery();
                }
            }

            return NoContent();
        }

        // 식당 삭제
        public IActionResult DeleteRestaurnat(int restaurantId)
        {
            if (restaurantId < 1)
                return BadRequest();

            var existsRow = _context.Restaurant.AsNoTracking()
                                              .Where(i => i.RestaurantId == restaurantId)
                                              .SingleOrDefault();

            if (existsRow != null)
            {
                _context.Restaurant.Remove(existsRow);
                _context.SaveChanges();
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }
        
        // 식당 수정
        public IActionResult PutRestaurnat(int restaurantId, string restaurantName)
        {
            if (restaurantId < 1)
                return BadRequest();

            var existsRow = _context.Restaurant.AsNoTracking()
                                               .Where(i => i.RestaurantId == restaurantId)
                                               .SingleOrDefault();

            if (existsRow != null)
            {
                existsRow.RestaurantName = restaurantName;

                _context.Restaurant.Update(existsRow);
                _context.SaveChanges();
            }

            return NoContent();
        }
        
        public void SetErrorMessages(ref Payload payload, Exception ex)
        {
            var trace = new StackTrace(ex);
            var assembly = Assembly.GetExecutingAssembly();
            var methodName = trace.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == assembly).Name;

            payload.ErrorMessages.Add(new string('-', 10));
            payload.ErrorMessages.Add($"## Exception from {methodName}");
            payload.ErrorMessages.Add($"## Exception Message {ex.Message}");

            
            if (ex.InnerException != null)
                payload.ErrorMessages.Add($"## Inner Exception Message {ex.InnerException.Message}");

            payload.ErrorMessages.Add(new string('-', 10));

            if (ex is WebException)
            {
                HttpWebResponse response = (HttpWebResponse)(ex as WebException).Response;
                payload.StatusCode = response.StatusCode;
            }

            Debug.WriteLine(string.Join(Environment.NewLine, payload.ErrorMessages.ToArray()));
        }
    }
}
