import { Component, OnInit } from '@angular/core';
import { Logger } from 'angular2-logger/core';
import { DeliveryService } from '../../../service/delivery.service';

@Component({
    selector: 'category-list',
    templateUrl: './category-list.screen.html',
    styleUrls: ['./category-list.screen.css']
})
export class CategoryListScreen implements OnInit {

    constructor(
        protected logger: Logger,
        private delivery: DeliveryService) {

    }

    ngOnInit(): void {
        this.logger.debug("[Pixstock][CategoryListScreen] ngOnInit");
        //throw new Error("Method not implemented.");
    }

    onClick_PreviewScreen() {
        this.delivery.showScreenPreview(0);
    }

    onClick_DebugCommand() {
        this.logger.debug("[Pixstock][CategoryListScreen] onClick_DebugCommand");
        this.delivery.executeDebugCommand("on CategoryList");
    }
}
