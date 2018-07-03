import { Injectable } from "@angular/core";
import { MessagingService } from "./messaging.service";
import { IpcUpdateViewResponse } from "./contract/response.contract";
import { Logger } from "angular2-logger/core";
import { Router } from "@angular/router";

@Injectable()
export class NaviService {
    /**
     * コンストラクタ
     * 
     * @param logger
     * @param messagingSrv
     */
    constructor(protected logger: Logger,
        protected messagingSrv: MessagingService,
        protected router: Router
    ) {

    }


    /**
     * 初期化メソッド
     */
    public initialize() {
        this.messagingSrv.UpdateView.subscribe((prop: IpcUpdateViewResponse) => {
            prop.UpdateList.forEach(item => {
                this.logger.info("[Pixstock][Navi][UpdateView] Navigation", item, prop.Parameter);
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

                // 複数の遷移アイテムが登録されている場合は、
                // ルーティング処理を複数回呼び出す。
                if (item.UpdateType == "PUSH") {
                    this.routing(item.ScreenName, prop.Parameter);
                } else if (item.UpdateType == "SET") {
                    this.routing(item.ScreenName, prop.Parameter);
                }
            });
        });
    }

    /**
     * Angularのルーティング処理
     * 
     * @param screenName ルーティング先スクリーン名（app.shared.module.tsで登録したパスを指定する）
     * @param param
     */
    private routing(screenName: string, param: any) {
        this.router.navigate(['/' + screenName], { queryParams: param });
    }
}
