import { Injectable } from "@angular/core";
import { ViewModel } from "./../viewmodel";
import { MessagingService } from "./messaging.service";
import { Logger } from "angular2-logger/core";
import { IntentMessage, IpcMessage } from "./contract/delivery.contract";

/**
 * BFFへインテント・メッセージを送信するサービス
 */
@Injectable()
export class DeliveryService {
    initializedFlag: boolean = false;

    /**
     * コンストラクタ
     *
     * @param logger ロガー
     * @param messaging BFF間のメッセージサービス
     */
    constructor(
        protected logger: Logger,
        protected messaging: MessagingService,
        protected viewModel: ViewModel) {
    }

    public initialize() {
        console.debug("initialize");
    }

    /**
     * プレビュー画面を表示する遷移イベントを発行します
     *
     * @param index 表示したいアイテムのナビゲーションリスト内での位置
     */
    public showScreenPreview(index: number) {
        var intentMessage = new IntentMessage();
        intentMessage.ServiceType = "Workflow";
        intentMessage.MessageName = "TRNS_PreviewPage";
        intentMessage.Parameter = index.toString();

        var ipcMessage = new IpcMessage();
        ipcMessage.Body = JSON.stringify(intentMessage);
        this.messaging.ipcRenderer.send("PIXS_INTENT_MESSAGE", ipcMessage);
    }
}
