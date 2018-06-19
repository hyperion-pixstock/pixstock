import { Component } from '@angular/core';
import { DeliveryService } from '../../../service/delivery.service';

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.screen.html',
    styleUrls: ['./dashboard.screen.css']
})
export class DashboardScreen {

    public constructor(private delivery: DeliveryService) {
        
    }

    onClick_TransTopScreen() {
        this.delivery.transTopScreen();
    }

    onClick_CategoryListTransitionSample() {
        // サンプル
        console.info("カテゴリ一覧画面への遷移メッセージ呼び出し");
        this.delivery.showScreenCategorytList();
    }
}
