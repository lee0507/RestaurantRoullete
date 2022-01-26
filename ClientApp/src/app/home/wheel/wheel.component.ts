import { AfterViewInit, ElementRef, OnDestroy } from '@angular/core';
import { ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgxWheelComponent } from 'ngx-wheel';
import { Subscription } from 'rxjs';
import { DataStorageService } from '../../shared/data-storage.service';
import { DeviceCheckService } from '../device-check.service';

declare let Winwheel: any;

@Component({
  selector: 'app-wheel',
  templateUrl: './wheel.component.html',
  styleUrls: ['./wheel.component.css']
})
export class WheelComponent implements OnInit, OnDestroy {
  @ViewChild(NgxWheelComponent, { static: false }) wheel: any;
  @ViewChild('f') slForm: NgForm;

  items: [{ id: number, text: string }] | any = [{ id: 12000, text: '' }];
  restaurantNameDisplay = false;
  deviceInfo = null;
  restaurantName = "";
  isLoading = false;
  isViewRoulette = false;
  isSpinning = false;
  isViewReset = false;
  isViewList = false;
  masterSelected: boolean;
  checklist: any;
  checkedList: any;

  subscription: Subscription;
  idToLandOn = this.items[Math.floor(Math.random() * this.items.length)].id;

  constructor(
    private dataStorageService: DataStorageService,
    private deviceCheckService: DeviceCheckService) {
    this.deviceInfo = deviceCheckService.epicFunction();
    console.log('dddd', this.deviceInfo)
    console.log('dddd', this.items[0].id)

    this.masterSelected = false;
    this.checklist = [
      { id: 1, text: '이동현1', isSelected: false },
      { id: 2, text: '이동현2', isSelected: true },
      { id: 3, text: '이동현3', isSelected: true },
      { id: 4, text: '이동현4', isSelected: true },
      { id: 5, text: '이동현5', isSelected: false },
      { id: 6, text: '이동현6', isSelected: false },
      { id: 7, text: '이동현7', isSelected: false },
      { id: 8, text: '이동현8', isSelected: false }
    ];
    this.getCheckedItemList();

  }

  ngOnInit(): void {
    console.log('init')
  }

  checkUncheckAll() {
    for (var i = 0; i < this.checklist.length; i++) {
      this.checklist[i].isSelected = this.masterSelected;
    }
    this.getCheckedItemList();
  }


  isAllSelected() {
    this.masterSelected = this.checklist.every(function (item: any) {
      return item.isSelected == true;
    })
    this.getCheckedItemList();
  }

  getCheckedItemList() {
    this.checkedList = [];
    for (var i = 0; i < this.checklist.length; i++) {
      if (this.checklist[i].isSelected)
        this.checkedList.push(this.checklist[i]);
    }

    this.items = this.checkedList.map(x => {
      return { ...x, fillStyle: "#" + Math.floor(Math.random() * 16777215).toString(16), textFillStyle: 'white' }
    });

    console.log(this.items, 'this.items');
    console.log(this.checkedList, 'this.checkedList');

  }

  //돌리면 바로 호출
  before() {
    console.log(this.items, 'before items')
  }

  //돌리는게 멈추면 호출
  after() {
    this.isViewReset = true;
    this.isViewList = false;

    this.restaurantNameDisplay = true;
    console.log('after', this.idToLandOn);
    this.restaurantName = this.items.find(x => x.id === this.idToLandOn).text;
    console.log(`당첨 ${this.restaurantName}`);

    //여기다 당첨 결과 DB로 보내는 코드 작성하면 됨(해야할 일)
    console.log('여기다가 post 배열 작성')
    this.DeleteRandomColor();
  }

  //세팅 버튼 누르면 호출
  myreset() {
    this.isSpinning = false;
    this.isViewReset = false;
    this.isViewRoulette = true;
    this.restaurantNameDisplay = false;
    console.log('myafter');
    this.idToLandOn = this.items[Math.floor(Math.random() * this.items.length)].id;
  }

  //돌리는 버튼
  spin() {
    this.isSpinning = true;
    this.isViewList = true;
    this.wheel.spin();
    console.log(this.items, 'spin')
  }

  reset() {
    this.isSpinning = false;
    this.isViewReset = false;
    this.restaurantNameDisplay = false;
    this.wheel.reset();
    this.idToLandOn = this.items[Math.floor(Math.random() * this.items.length)].id;
  }

  list() {
    this.isViewRoulette = false;
  }

  //색상 삭제하는 함수(해야할 일)
  DeleteRandomColor() {

  }

  deleteRestaurant(itemId) {
    if (Object.keys(this.items).length > 1) {
    } else {
      alert('식당이 하나 이상 있어야합니다.')
    }
    console.log(this.items, 'delete');
  }

  //프론트에서 백엔드로 데이터를 보낼때에는 text만 보낸다. 여기서 길이로 계산해서 저장하려고 하니 중복이 생김
  //비동기로 데이터를 세팅하고 길이로 보내면 프론트에서 id 붙여서 보낼 수도 있을듯
  addRestaurant(form: NgForm) {
    const NewForm = {
      id: this.checklist[this.checklist.length - 1].id + 1,
      text: form.value.text,
      isSelected: true
    };
    this.checklist.push(NewForm)
    this.checkedList.push(NewForm)
    this.getCheckedItemList();

    form.reset();
  }

  addMenuSetting() {
    let checkBoxId = document.getElementById("")
  }

  ngOnDestroy() {
    //this.subscription.unsubscribe();
  }
}
