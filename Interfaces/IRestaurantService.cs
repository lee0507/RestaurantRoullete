using Borago.Models;
using Microsoft.AspNetCore.Mvc;
using Swilago.Models;

namespace Swilago.Interfaces
{
    public interface IRestaurantService
    {
        // 전체 조회
        Payload GetRestaurantList();

        // 하나만 조회
        Payload GetRestaurant(int restaurantId);

        // 식당 추가
        IActionResult AddRestaurant(string restaurantName);

        // 룰렛 당첨결과 저장
        IActionResult AddRouletteResult(string rouletteResult);

        // 룰렛 리스트 최신화
        IActionResult AddRouletteList(List<RouletteList> jsonRouletteResultList);

        // 식당 삭제
        IActionResult DeleteRestaurnat(int restaurantId);
        
        // 식당 수정
        IActionResult PutRestaurnat(int restaurantId, string restaurantName);
    }
}
