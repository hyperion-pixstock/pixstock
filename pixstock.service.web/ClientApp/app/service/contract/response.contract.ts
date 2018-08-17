import { Content } from "../../model/content.model";
import { Category } from "../../model/category.model";

export interface IpcResponse {
  body: string;
}


export interface CategoryDetailResponse {
  Category: Category;
  SubCategory: Category[];
  Content: Content[];
}

export interface ContentDetailResponse {
  Content: Content;
  Category: Category;
}


/**
 * IPC_UPDATEPROPイベントのパラメータ
 */
export interface IpcUpdatePropResponse {
  PropertyName: string;
  Hint: string;
  Value: any;
}

/**
 * PropertyNameがCategoryList
 */
export interface CategoryListUpdateProp {
  CategoryList: Category[];
}

/**
 *
 */
export interface PreviewContentProp {
  Content: Content;
  Category: Category;
}

/**
 * "IPC_UPDATEVIEW"メッセージの本文
 */
export interface IpcUpdateViewResponse {
  /** 表示更新リスト */
  UpdateList: UpdateViewRequestItem[];

  /** パラメータ */
  Parameter: object;

  /** 遷移先画面名称 */
  NextScreenName: string; // 空文字の場合もあります
}

export interface UpdateViewRequestItem {
  ScreenName: string;
  UpdateType: string;
}
