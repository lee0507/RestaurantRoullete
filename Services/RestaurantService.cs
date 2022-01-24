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
        public Payload GetRestaurantList()
        {
            Payload payload = new();

            try
            {
                Restaurants restaurants = new();
                using (SqlConnection dbConnection = new(_config["SqlServerConnection"]))
                {
                    dbConnection.Open();
                    using (SqlCommand command = dbConnection.CreateCommand())
                    {
                        command.CommandTimeout = 0;
                        command.CommandText = @"SELECT * FROM dbo.T_Restaurant";
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                restaurants.RestaurantList.Add(
                                    new Restaurant()
                                    {
                                        id = (int)reader["RestaurantId"],
                                        text = (string)reader["RestaurantName"]
                                    }
                                );
                            }
                        }
                    }
                }
                
                payload.Message = JsonConvert.SerializeObject(restaurants, Formatting.Indented);
            }
            catch (Exception ex)
            {
                SetErrorMessages(ref payload, ex);
            }

            return payload;
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

        // 식당 추가
        public IActionResult AddRestaurant(string restaurantName)
        {
            if (string.IsNullOrEmpty(restaurantName))
                return BadRequest();

            TRestaurant newRestaurant = new()
            {
                RestaurantName = restaurantName
            };

            _context.Restaurant.Add(newRestaurant);
            _context.SaveChanges();

            return NoContent();
        }

        // 룰렛 당첨결과 저장
        public IActionResult AddRouletteResult(string rouletteResult)
        {
            if (string.IsNullOrEmpty(rouletteResult))
                return BadRequest();

            // T_Statistics 테이블이 비었으면 1로 초기화
            int statisticsId = 1;

            // 비어있지 않으면 마지막 번호에서 1을 추가한 값으로 초기화
            if (_context.Statistics.Any())
            {
                var lastRow = _context.Statistics.AsNoTracking().OrderByDescending(c => c.StatisticsId).FirstOrDefault();
                statisticsId = lastRow.StatisticsId;
                statisticsId++;
            }

            // UserAccessInfo에서 email 가져오기
            //===========================================================================================================================================================================================

            //===========================================================================================================================================================================================

            TStatistics statistics = new()
            {
                StatisticsId = statisticsId,
                //Email = email,
                RouletteResult = rouletteResult,
                ModifiedDate = DateTime.Now
            };

            _context.Statistics.Add(statistics);
            _context.SaveChanges();
            
            return NoContent();
        }

        // 룰렛 리스트 최신화
        public IActionResult AddRouletteList(List<RouletteList> jsonRouletteResultList) // List: { id, text, fillStyle, textFillStyle }
        {
            if (jsonRouletteResultList == null)
                return BadRequest();

            int rouletteResultListCount = jsonRouletteResultList.Count;

            // rouletteResultList에서 text만 따로 뽑기
            string restaurantList = "";
            foreach (var jsonRouletteResult in jsonRouletteResultList)
            {
                //string jsonString = System.Text.Json.JsonSerializer.Serialize(jsonRouletteResult); // { "id", "text", "fillStyle", "textFillStyle" }
                //var jsonKey = JsonConvert.DeserializeObject<RouletteList>(jsonRouletteResult); // insert jsonString to Models.RouletteResultList.cs
                restaurantList += (jsonRouletteResult.text + @"/split/");
            }
            
            // T_Restaurant 변경
            using (SqlConnection dbConnection = new(_config["SqlServerConnection"]))
            {
                dbConnection.Open();
                using (SqlCommand command = dbConnection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = "EXEC dbo.P_ChangeRestaurantTableToRouletteResultList @RouletteResultListCount, @RestaurantList";

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
