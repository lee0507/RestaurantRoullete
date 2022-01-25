import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { map, tap, filter } from "rxjs/operators";

import { Restaurant } from "../shared/restaurant.model";

//데이터는 여기서 다 관리하고 컴포넌트에서는 함수만 불러서 사용하게 할 것.
//비동기로 하려면 그렇게 하는게 맞을 거 같다.

@Injectable()
export class HomeService {
  itemsChanged = new Subject<any[]>();

  items: [{ id: number, text: string }] | any = [];

  constructor(private http: HttpClient) { }

  setRestaurant(items) {
    this.items = items;
    this.itemsChanged.next(this.items.slice());
  }

  getRestaurants() {
    return this.items.slice();
  }

  addRestaurant(items) {
    this.items.push(items);
    this.itemsChanged.next(this.items.slice());
    console.log(this.items, 'Home addItems')
  }

  //updateRestaurant(index: number, newIngredient: Restaurant) {
  //  this.items[index] = newIngredient;
  //  this.itemsChanged.next(this.items.slice());
  //}

  deleteRestaurant(index: number) {
    const newData = this.items.filter((item) => item.id !== index);
    this.items = newData;
    this.itemsChanged.next(this.items.slice());
    console.log(this.items, 'Home deleteItems', newData)
  }

}
