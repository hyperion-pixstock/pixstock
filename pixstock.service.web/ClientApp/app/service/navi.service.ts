import { Injectable } from "@angular/core";
import { MessagingService } from "./messaging.service";
import { IpcUpdateViewResponse } from "./contract/response.contract";
import { Logger } from "angular2-logger/core";

@Injectable()
export class NaviService {
    /**
     * コンストラクタ
     * 
     * @param logger
     * @param messagingSrv
     */
    constructor(protected logger: Logger,
        protected messagingSrv: MessagingService) {

    }

    /**
     * 初期化
     */
    public initialize() {
        this.messagingSrv.UpdateView.subscribe((prop: IpcUpdateViewResponse) => {
            prop.UpdateList.forEach(item => {
                this.logger.info("[NaviService][UpdateView] Navigation", item, prop.Parameter);
                // var navCtrl = this.app.getActiveNav();// ftom ionic
                /*
                // デバッグ用
                if (prop.Parameter.toString() == "TRNS_DEBUG_BACK") {
                    navCtrl.setRoot("HomePage");
                    return;
                }

                if (item.UpdateType == "PUSH") {
                    navCtrl.push(item.ScreenName, prop.Parameter); // AppModuleのDeepLinksで設定したスクリーン名を指定する
                } else if (item.UpdateType == "POP") {
                    navCtrl.pop();
                } else if (item.UpdateType == "SET") {
                    navCtrl.setRoot(item.ScreenName, prop.Parameter);
                }
                */
            });
        });
    }
}
