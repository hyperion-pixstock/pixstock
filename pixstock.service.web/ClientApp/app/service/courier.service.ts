import { Injectable } from "@angular/core";
import { IpcUpdatePropResponse, CategoryListUpdateProp, PreviewContentProp } from "./contract/response.contract";
import { ViewModel, ThumbnailListPageItem, ContentListPageItem } from "../viewmodel";
import { Category } from "../model/category.model";
import { Content } from "../model/content.model";
import { BehaviorSubject } from "../../../node_modules/rxjs";

/**
 * クーリエサービス
 */
@Injectable()
export class CourierService {
  initializedFlag: boolean = false;

  // IPC_INVALIDATEPROPメッセージの内部通知用イベント
  invalidateProp$ = new BehaviorSubject<IpcUpdatePropResponse>(undefined);

  /**
   *
   */
  private internalIpcUpdatePropResponse: IpcUpdatePropResponse;

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

  /**
   * コンストラクタ
   */
  constructor(
    protected viewModel: ViewModel
  ) {
    // NOTE: 各イベントのsubscribeは、ここで追加する（メンバ化しない）

    this.invalidateProp$.subscribe((response: IpcUpdatePropResponse) => {
      console.debug("[Pixstock][Messaging][onInvalidateProp] IN");
      if (response == undefined) return;

      switch (response.PropertyName) {
        case "CategoryTree":
          console.debug("[Pixstock][Messaging][onInvalidateProp] CategoryTreeプロパティ更新");
          break;
        case "ContentList":
          let objValue = JSON.parse(response.Value) as Content[];
          if (objValue != null) {
            console.debug("[Pixstock][Messaging][onInvalidateProp] ContentList", this.viewModel);
            this.updateContentList(objValue);
          } else {
            console.warn("プロパティを正常に復号化できませんでした");
          }
          break;
        default:
          // DEBUG:
          break;
      }
    });
  }

  /**
   * 初期化
   */
  public initialize() {
    if (this.initializedFlag) return;
    this.initializedFlag = true;
    /*
    this.messagingSrv.UpdateProp.subscribe((prop: IpcUpdatePropResponse) => {
      console.info("[Pixstock][Courier][UpdateProp.subscribe] レジスタ更新 レジスタ名=" + prop.PropertyName, prop.Value);

      if (prop.PropertyName == "CategoryList") {
        let objValue = JSON.parse(prop.Value) as CategoryListUpdateProp;
        if (objValue != null) {
          this.updateCategoryList(objValue);
        } else {
          console.warn("プロパティを正常に復号化できませんでした");
        }
        this.viewModel.CategoryListLazyLoadSpinner = false;
      } else if (prop.PropertyName == "ContentList") {
        let objValue = JSON.parse(prop.Value) as ContentListUpdateProp;
        if (objValue != null) {
          this.updateContentList(objValue);
        } else {
          console.warn("プロパティを正常に復号化できませんでした");
        }
      } else if (prop.PropertyName == "PreviewContent") {
        let objValue = JSON.parse(prop.Value) as PreviewContentProp;
        if (objValue != null) {
          this.viewModel.PreviewContent = objValue.Content;
          this.viewModel.PreviewCategory = objValue.Category;
        } else {
          console.warn("プロパティを正常に復号化できませんでした");
        }
      }

      // TODO: ここに、各プロパティ名の処理を追加します
    });
    */
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
   * ViewModelを更新します
   *
   * @param objValue
   */
  private updateCategoryList(objValue: CategoryListUpdateProp) {
    console.debug("[updateCategoryList] カテゴリ一覧を更新しました");

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
  private updateContentList(objValue: Content[]) {
    console.debug("[updateContentList] コンテント一覧を更新しました", objValue);

    this.viewModel.ContentListPageItem = [];

    objValue.forEach((inprop: Content) => {
      let listitem = {} as ContentListPageItem;
      listitem.Content = inprop;

      this.viewModel.ContentListPageItem.push(listitem);
    });
  }
}
