import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { AuthenticationResult, EventMessage, EventType, InteractionStatus } from '@azure/msal-browser';
import { filter, map } from 'rxjs/operators';
import { NgxWheelComponent } from 'ngx-wheel';
import { ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { HomeService } from './home.service';
import { DeviceCheckService } from './device-check.service';
import { DataStorageService } from '../shared/data-storage.service';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  loginDisplay = false;


  constructor(
    private authService: MsalService,
    private msalBroadcastService: MsalBroadcastService,
    private dataStorageService: DataStorageService  ) {

  }

  ngOnInit(): void {
    this.msalBroadcastService.msalSubject$
      .pipe(
        filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS),
      )
      .subscribe((result: EventMessage) => {
        console.log(result,"post로 백엔드에 보내야 할 것");
        //result 내부에 토큰이랑 유저 정보 들어있음, 백엔드에 post로 보내야함(해야할 일)
        const payload = result.payload as AuthenticationResult;
        this.authService.instance.setActiveAccount(payload.account);

        this.dataStorageService.postLoginInfo(payload);

      });

    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None)
      )
      .subscribe(() => {
        this.setLoginDisplay();
      })
  }

  setLoginDisplay() {
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
  }
}
