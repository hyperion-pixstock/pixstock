import { Injectable } from "@angular/core";
import { IpcUpdatePropResponse, CategoryListUpdateProp, PreviewContentProp, IpcUpdateViewResponse } from "./contract/response.contract";
import { ViewModel, ThumbnailListPageItem, ContentListPageItem } from "../viewmodel";
import { Category } from "../model/category.model";
import { Content } from "../model/content.model";
import { BehaviorSubject } from "../../../node_modules/rxjs";
import { DomSanitizer } from "@angular/platform-browser";

/**
 * クーリエサービス
 */
@Injectable()
export class CourierService {
  private LOGEVENT: string = "[Pixstock][CourierService]";

  /**
   *  IPC_INVALIDATEPROPメッセージの内部通知用イベント
   */
  invalidateProp$ = new BehaviorSubject<IpcUpdatePropResponse>(undefined);

  /**
   * IPC_UPDATEVIEWメッセージの内部通知用イベント
   */
  updateView$ = new BehaviorSubject<IpcUpdateViewResponse>(undefined);

  /** */
  private internalIpcUpdatePropResponse: IpcUpdatePropResponse;

  /** */
  private internalIpcUpdateViewResponse: IpcUpdateViewResponse;

  /**
   * invalidatePropイベントのパラメータを取得する
   */
  private get invalidateObject(): IpcUpdatePropResponse { return this.internalIpcUpdatePropResponse; }

  /**
   * invalidatePropイベントを発火する
   */
  private set invalidateObject(response: IpcUpdatePropResponse) {
    this.invalidateProp$.next(response);
    this.internalIpcUpdatePropResponse = response;
  }

  private get updateView(): IpcUpdateViewResponse { return this.internalIpcUpdateViewResponse; }

  private set updateView(response: IpcUpdateViewResponse) {
    this.updateView$.next(response);
    this.internalIpcUpdateViewResponse = response;
  }

  /**
   * コンストラクタ
   */
  constructor(
    private viewModel: ViewModel,
    private sanitizer: DomSanitizer
  ) {
    // NOTE: 各イベントのsubscribeは、ここで追加する（メンバ化しない）

    this.invalidateProp$.subscribe((response: IpcUpdatePropResponse) => {
      console.debug(this.LOGEVENT, "[onInvalidateProp] IN");
      if (response == undefined) return;

      switch (response.PropertyName) {
        case "CategoryTree":
          console.debug(this.LOGEVENT, "[onInvalidateProp] CategoryTreeプロパティ更新");
          break;
        case "ContentList":
          console.debug(this.LOGEVENT, "[onInvalidateProp] ContentList");
          let objValue = JSON.parse(response.Value) as ContentListParam;
          if (objValue != null) {
            this.updateContentList(objValue);
          } else {
            console.warn("プロパティを正常に復号化できませんでした");
          }
          break;
        case "PreviewUrl":
          this.previewUrl(response);
          break;
        default:
          console.warn(this.LOGEVENT, "[invalidateProp$] 処理できないプロパティ名", response.PropertyName);
          break;
      }
    });
  }

  /**
   * InvalidateObjectイベントを発火する
   *
   * @param eventArgs
   */
  fireInvalidateProp(eventArgs: IpcUpdatePropResponse) {
    this.invalidateObject = eventArgs;
  }

  /**
   * UpdateViewイベントを発火します
   *
   * @param eventArgs
   */
  fireUpdateView(eventArgs: IpcUpdateViewResponse) {
    this.updateView = eventArgs;
  }

  /**
   * ViewModelを更新します
   *
   * @param objValue
   */
  private updateCategoryList(objValue: CategoryListUpdateProp) {
    console.debug(this.LOGEVENT, "[updateCategoryList] カテゴリ一覧を更新しました");

    objValue.CategoryList.forEach((inprop: Category) => {
      let listitem = {} as ThumbnailListPageItem;
      listitem.Selected = false;
      listitem.Category = inprop;
      listitem.IsContent = true;
      if (inprop.HasLinkSubCategoryFlag) {
        listitem.IsSubCaetgory = true;
      } else {
        listitem.IsSubCaetgory = false;
      }
      this.viewModel.ThumbnailListPageItem.push(listitem);
    });
  }

  /**
   * ViewModelを更新します
   *
   * @param objValue
   */
  private updateContentList(objValue: ContentListParam) {
    console.debug(this.LOGEVENT, "[updateContentList] コンテント一覧を更新しました", objValue);

    this.viewModel.ContentListPageItem = [];

    objValue.ContentList.forEach((inprop: Content) => {
      let listitem = {} as ContentListPageItem;
      listitem.ThumbnailUrl = inprop.ThumbnailImageSrcUrl;
      listitem.Content = inprop;

      this.viewModel.ContentListPageItem.push(listitem);
    });
  }

  /**
   * PreviewUrlプロパティの更新
   * @param response
   */
  private previewUrl(response: IpcUpdatePropResponse) {
    let objValue = JSON.parse(response.Value) as string;
    this.viewModel.PreviewUrl = this.sanitizer.bypassSecurityTrustUrl(objValue);
  }

}

/**
 * プロパティデータ更新メッセージによるContentList更新で受け取るパラメータ
 */
interface ContentListParam {
  Category: Category | null;
  ContentList: Content[];
}
