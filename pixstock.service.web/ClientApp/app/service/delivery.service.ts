import { Injectable } from "@angular/core";
import { ViewModel } from "./../viewmodel";
import { MessagingService } from "./messaging.service";
import { IntentMessage, IpcMessage } from "./contract/delivery.contract";

/**
 * BFFへインテント・メッセージを送信するサービス
 */
@Injectable()
export class DeliveryService {
  private LOGEVENT: string = "[Pixstock][DeliveryService]";

  /**
   * コンストラクタ
   *
   * @param messaging BFF間のメッセージサービス
   * @param viewModel
   */
  constructor(
    protected messaging: MessagingService,
    protected viewModel: ViewModel) {
  }

  /**
   * 遷移図の初期化用メッセージ呼び出し.
   *
   * クライアントが利用可能であることを、BFFに通知するためのイベントです
   */
  public transTopScreen() {
    console.info("[Pixstock][Delivery][transTopScreen] 遷移メッセージ送信");
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
    console.info("[Pixstock][Delivery][backScreen]");
    var intentMessage = new IntentMessage();
    intentMessage.ServiceType = "Workflow";
    intentMessage.MessageName = "TRNS_BACK";
    intentMessage.Parameter = "";

    this.send(intentMessage);
  }

  /**
   * デバッグ用遷移メッセージを発行します
   */
  public transRootBack() {
    var intentMessage = new IntentMessage();
    intentMessage.ServiceType = "Workflow";
    intentMessage.MessageName = "TRNS_DEBUG_BACK";
    intentMessage.Parameter = "";

    this.send(intentMessage);
  }

  /**
   * カテゴリ一覧画面を表示する遷移イベントを発行します
   */
  public showFinder() {
    console.info("[Pixstock][Delivery][showFinder] カテゴリ一覧画面への遷移メッセージ送信");
    var intentMessage = new IntentMessage();
    intentMessage.ServiceType = "Workflow";
    intentMessage.MessageName = "TRNS_FinderScreen";
    intentMessage.Parameter = "";

    this.send(intentMessage);
  }

  /**
   * プレビュー画面を表示する遷移イベントを発行します
   *
   * @param index 表示したいアイテムのナビゲーションリスト内での位置
   */
  public showScreenPreview(index: number) {
    console.info("[Pixstock][Delivery][showScreenPreview] プレビュー画面への遷移メッセージ送信", "コンテントリストの選択", index);
    var intentMessage = new IntentMessage();
    intentMessage.ServiceType = "Workflow";
    intentMessage.MessageName = "TRNS_ContentListPreview";
    intentMessage.Parameter = index.toString();

    this.send(intentMessage);
  }

  /**
   * カテゴリ一覧の更新を行います
   *
   * @param categoryId カテゴリツリーを取得したい親カテゴリのID
   */
  public updateCategoryTree(categoryId: number) {
    console.info("[Pixstock][Delivery][updateCategoryTree] ", );
    var intentMessage = new IntentMessage();
    intentMessage.ServiceType = "Workflow";
    intentMessage.MessageName = "ACT_REQINVALIDATE_CATEGORYTREE";
    intentMessage.Parameter = categoryId.toString();

    this.send(intentMessage);
  }

  /**
   * コンテント一覧を更新します
   *
   * @param categoryId
   */
  public updateContentList(categoryId: number) {
    // ルール: 任意のカテゴリに紐付けられているコンテント一覧を作成します。
    // TODO: 他のルールも追加する
    console.info("[Pixstock][Delivery][updateContentList] ", categoryId);

    var intentMessage = new IntentMessage();
    intentMessage.ServiceType = "Workflow";
    intentMessage.MessageName = "ACT_REQINVALIDATE_CONTENTLIST";
    intentMessage.Parameter = JSON.stringify({ ContentId: categoryId });

    this.send(intentMessage);
  }



  /**
   * プレビュー画面の表示内容を更新する
   *
   * @param position コンテント一覧の位置
   */
  public invalidatePreviewContentList(position: number) {
    // プレビューコンテント更新要求メッセージ(ACT_REQINVALIDATE_PREVIEW)の送信用メソッド
    console.debug(this.LOGEVENT, "[invalidatePreviewContentList] - IN");

    var intentMessage = new IntentMessage();
    intentMessage.ServiceType = "Workflow";
    intentMessage.MessageName = "ACT_REQINVALIDATE_PREVIEW";
    intentMessage.Parameter = JSON.stringify({
      Operation: "NavigationPosition",
      Position: position
    });
    this.send(intentMessage);

    console.debug(this.LOGEVENT, "[invalidatePreviewContentList] - OUT");
  }

  /**
   * プレビュー画面の表示内容を更新する
   *
   * コンテントIDに指定したコンテントで更新する。
   *
   * @param contentId コンテントID
   */
  public invalidatePreviewContentId(contentId:number) {
    // TODO:
  }

  public executeDebugCommand(command: string) {
    console.info("[Pixstock][Delivery][executeDebugCommand] デバッグコマンドメッセージ送信", command);
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
