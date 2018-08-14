import { Injectable } from "@angular/core";
import { Category } from "./model/category.model";
import { Content } from "./model/content.model";
import { SafeUrl } from "@angular/platform-browser";

/**
 * ビューモデル
 */
@Injectable()
export class ViewModel {
  /** 画面表示状態 */
  screenStatus: ScreenStatus = new ScreenStatus();

  /**
   * サムネイル一覧画面のサムネイル付きカテゴリ情報リスト
   */
  ThumbnailListPageItem: ThumbnailListPageItem[] = [];

  /**
   * サムネイル一覧画面のコンテント情報リスト
   */
  ContentListPageItem: ContentListPageItem[] = [];

  /**
   * サムネイル一覧画面のカテゴリリストで、遅延読み込み時のスピナ表示の有無を設定する
   */
  CategoryListLazyLoadSpinner: boolean = true;

  /**
   * プレビュー画面に表示するコンテント情報
   */
  PreviewContent: Content | null = null;

  /**
   * プレビュー画面に表示しているコンテント情報が所属するカテゴリ情報
   */
  PreviewCategory: Category | null = null;

  /**
   * プレビュー画面の画像URL
   */
  PreviewUrl: SafeUrl = null;
}

/**
 * 画面表示状態
 */
export class ScreenStatus {
  mPreviewParam: PreviewParam = null;

  /**
   * ダッシュボード画面の表示を行う
   */
  showDashboard() {
    this.clear();
    this.dashboard = true;
  }

  /**
   * ファインダ画面の表示を行う
   */
  showFinder() {
    this.clear();
    this.finder = true;
  }

  /**
   * プレビュー画面の表示を行う
   *
   * @param param プレビュー画面に渡すパラメータ(PreviewParam対応型)
   */
  showPreview(param: object) {
    let paramObj = param as PreviewParam;
    this.mPreviewParam = paramObj;

    this.clear();
    this.preview = true;
  }

  /**
   *
   */
  showPreviewContentList() {
    this.clear();
    this.preview_contentlist = true;
  }

  private clear() {
    this.dashboard = false;
    this.preview = false;
    this.preview_contentlist = false;
    this.finder = false;
  }

  /** ダッシュボード画面 */
  private dashboard: boolean = false;

  /** プレビュー画面 */
  private preview: boolean = false;

  /** コンテントリストプレビュー画面 */
  private preview_contentlist: boolean = false;

  /** 一覧画面 */
  private finder: boolean = false;
}

export interface ThumbnailListPageItem {
  Selected: boolean;
  Category: Category;
  IsContent: boolean;
  IsSubCaetgory: boolean;
}

export interface ContentListPageItem {
  ThumbnailUrl: string;
  Content: Content;
}

export interface PreviewParam {
  Position: number | null;
}
