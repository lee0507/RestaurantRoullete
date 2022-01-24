using Borago.Models;
using Microsoft.AspNetCore.Mvc;
using Swilago.Interfaces;
using Swilago.Models;
using System.Text;

namespace Swilago.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _service;

        public RestaurantController(IRestaurantService service)
        {
            _service = service;
        }

        // 전체조회: /Restaurant/GetRestaurantList
        [HttpGet]
        [Route("GetRestaurantList")]
        public IActionResult GetRestaurantList()
        {
            return CheckErrors(_service.GetRestaurantList());
        }

        // 단일조회: /Restaurant/GetRestaurant?restaurantId=
        [HttpGet]
        [Route("GetRestaurant")]
        public IActionResult GetRestaurant(int restaurantId)
        {
            return CheckErrors(_service.GetRestaurant(restaurantId));
        }

        // 추가: /Restaurant/PostRestaurant?restaurantName=
        [HttpPost]
        [Route("PostRestaurant")]
        public IActionResult PostRestaurant(string restaurantName)
        {
            return _service.AddRestaurant(restaurantName);
        }

        // 룰렛 당첨결과 저장 : /Restaurant/PostRouletteResult?rouletteResult=
        [HttpPost]
        [Route("PostRouletteResult")]
        public IActionResult PostRouletteResult(string rouletteResult)
        {
            return _service.AddRouletteResult(rouletteResult);
        }

        // 룰렛 리스트 최신화 : /Restaurant/PostRouletteList
        // ex) jsonRouletteResultList = { id, text, fillStyle, textFillStyle } is in Body
        [HttpPost]
        [Route("PostRouletteList")]
        public IActionResult PostRouletteResult([FromBody] List<RouletteList> jsonRouletteResultList)
        {
            return _service.AddRouletteList(jsonRouletteResultList);
        }

        // 삭제: /Restaurant/DeleteRestaurant?restaurantId=
        [HttpDelete]
        [Route("DeleteRestaurant")]
        public IActionResult DeleteRestaurant(int restaurantId)
        {
            return _service.DeleteRestaurnat(restaurantId);
        }

        // 수정: /Restaurant/PutRestaurant?restaurantId=&restaurantName=
        [HttpPut]
        [Route("PutRestaurant")]
        public IActionResult PutRestaurant(int restaurantId, string restaurantName)
        {
            return _service.PutRestaurnat(restaurantId, restaurantName);
        }

        public IActionResult CheckErrors(Payload payload)
        {
            if (payload == null)
                return Ok();

            if (payload.ErrorMessages.Count > 0)
                return StatusCode((int)payload.StatusCode,
                                  string.Join(Environment.NewLine, payload.ErrorMessages.ToArray()));

            return Content(payload.Message, "application/json", Encoding.UTF8);
        }
    }
}
