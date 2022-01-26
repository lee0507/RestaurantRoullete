import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map, tap } from "rxjs/operators";
import { HomeService } from "../home/home.service";

@Injectable({ providedIn: 'root' })
export class DataStorageService {
  constructor(private http: HttpClient, private homeService: HomeService) { }

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  }

  fetchRestaurant() {
    let url = '/Restaurant/GetRestaurantList';

    return this.http.get(url)
      .pipe(
        map(responseData => {
          const postArray = responseData["RestaurantList"];
          console.log(responseData["RestaurantList"], 'rlsasdfsadf')

          //for (let i = 0; i < postArray.length; i++) {
          //  console.log(postArray[i]);
          //  postArray[i]["fillStyle"] = "#" + Math.floor(Math.random() * 16777215).toString(16)
          //  postArray[i]["textFillStyle"] = 'white'
          //}

          console.log(postArray, 'postArray')
          return postArray;
        })
      )
  }

  postRestaurant(Result) {
    console.log(Result, 'Result');
    let url = '/Restaurant/PostRestaurantList?rouletteResult=' + Result;
    const items = this.homeService.getRestaurants();
    console.log(items, 'itmes')
    return this.http.post(url, items, this.httpOptions)
      .subscribe(responseData => {
        console.log(responseData, 'leeeeeee')
      })
  }

  postLoginInfo(payload) {
    const url = '';
    this.http.post(url, JSON.stringify(payload), this.httpOptions);
  }

}
