import { Injectable } from "@angular/core";
import { MessagingService } from "./messaging.service";
import { IpcUpdatePropResponse, CategoryListUpdateProp, ContentListUpdateProp, PreviewContentProp } from "./contract/response.contract";
import { ViewModel, ThumbnailListPageItem, ContentListPageItem } from "../viewmodel";
import { Category } from "../model/category.model";
import { Content } from "../model/content.model";

/**
 * クーリエサービス
 */
@Injectable()
export class CourierService {
  initializedFlag: boolean = false;

  /**
   * コンストラクタ
   *
   * @param messagingSrv BFF間のメッセージサービス
   */
  constructor(
    protected messagingSrv: MessagingService,
    protected viewModel: ViewModel) {
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

  private updateContentList(objValue: ContentListUpdateProp) {
    console.debug("[updateContentList] コンテント一覧を更新しました");

    this.viewModel.ContentListPageItem = [];

    objValue.ContentList.forEach((inprop: Content) => {
      let listitem = {} as ContentListPageItem;
      listitem.Content = inprop;

      this.viewModel.ContentListPageItem.push(listitem);
    });
  }
}
