import { Component, OnInit } from '@angular/core';

@Component({
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent  implements OnInit {
  showAccelerationBanner = '1';

  hideMessage() {
    localStorage.setItem('showAccelerationBanner', '0');

    this.showAccelerationBanner = '0';
  }

  ngOnInit(): void {
    this.showAccelerationBanner = localStorage.getItem('showAccelerationBanner') ?? '1';
  }
}
