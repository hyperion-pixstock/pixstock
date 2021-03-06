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
     * 遷移図の初期化用メッセージ呼び出し.
     *
     * クライアントが利用可能であることを、BFFに通知するためのイベントです
     */
    public transTopScreen() {
        this.logger.info("[Pixstock][Delivery][transTopScreen] 遷移メッセージ送信");
        var intentMessage = new IntentMessage();
        intentMessage.ServiceType = "Workflow";
        intentMessage.MessageName = "TRNS_TOPSCREEN";
        intentMessage.Parameter = "";

        this.send(intentMessage);
    }

    /**
     * 戻る遷移コマンドを発行します
     */
    public backScreen() {
        this.logger.info("[Pixstock][Delivery][backScreen]");
        var intentMessage = new IntentMessage();
        intentMessage.ServiceType = "Workflow";
        intentMessage.MessageName = "ACT_BACKSCREEN";
        intentMessage.Parameter = "";

        this.send(intentMessage);
    }

    /**
     * カテゴリ一覧画面を表示する遷移イベントを発行します
     */
    public showScreenCategorytList() {
        this.logger.info("[Pixstock][Delivery][showScreenCategorytList] カテゴリ一覧画面への遷移メッセージ送信");
        var intentMessage = new IntentMessage();
        intentMessage.ServiceType = "Workflow";
        intentMessage.MessageName = "TRNS_ThumbnailListPage";
        intentMessage.Parameter = "";

        this.send(intentMessage);
    }

    /**
     * プレビュー画面を表示する遷移イベントを発行します
     *
     * @param index 表示したいアイテムのナビゲーションリスト内での位置
     */
    public showScreenPreview(index: number) {
        this.logger.info("[Pixstock][Delivery][showScreenPreview] プレビュー画面への遷移メッセージ送信", index);
        var intentMessage = new IntentMessage();
        intentMessage.ServiceType = "Workflow";
        intentMessage.MessageName = "TRNS_PreviewPage";
        intentMessage.Parameter = index.toString();

        this.send(intentMessage);
    }

    /**
     * カテゴリ一覧の更新を行います
     */
    public updateCategoryTree() {
        this.logger.info("[Pixstock][Delivery][updateCategoryTree] ", );
        var intentMessage = new IntentMessage();
        intentMessage.ServiceType = "Workflow";
        intentMessage.MessageName = "ACT_CATEGORYTREE_UPDATE";
        intentMessage.Parameter = "1"; // TODO: パラメータの値は仮のもの。

        this.send(intentMessage);
    }

    public executeDebugCommand(command: string) {
        this.logger.info("[Pixstock][Delivery][executeDebugCommand] デバッグコマンドメッセージ送信", command);
        var intentMessage = new IntentMessage();
        intentMessage.ServiceType = "Workflow";
        intentMessage.MessageName = "ACT_DEBUGCOMMAND";
        intentMessage.Parameter = command;

        this.send(intentMessage);
    }

    private send(message: IntentMessage) {
        var ipcMessage = new IpcMessage();
        ipcMessage.Body = JSON.stringify(message);
        this.messaging.ipcRenderer.send("PIXS_INTENT_MESSAGE", ipcMessage);
    }
}
