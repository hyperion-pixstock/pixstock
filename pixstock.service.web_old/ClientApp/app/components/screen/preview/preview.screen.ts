import { Component } from '@angular/core';
import { DeliveryService } from '../../../service/delivery.service';
import { Logger } from 'angular2-logger/core';

@Component({
    selector: 'preview',
    templateUrl: './preview.screen.html',
    styleUrls: ['./preview.screen.css']
})
export class PreviewScreen {
    constructor(
        protected logger: Logger,
        private delivery: DeliveryService) {

    }

    onClick_Back() {
        this.logger.debug("[Pixstock][PreviewScreen] onClick_Back");

        // 戻る遷移
        this.delivery.backScreen();
    }

    onClick_DebugCommand() {
        this.logger.debug("[Pixstock][PreviewScreen] onClick_DebugCommand");
        this.delivery.executeDebugCommand("on CategoryList");
    }
}
