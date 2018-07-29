import { CollectionViewer, SelectionChange } from "@angular/cdk/collections";
import { FlatTreeControl } from "@angular/cdk/tree";
import { Component, Injectable } from "@angular/core";
import { BehaviorSubject, Observable, merge } from "rxjs";
import { map } from "rxjs/operators";
import { Category } from "../../../model/category.model";
import { IpcUpdatePropResponse } from "../../../service/contract/response.contract";
import { DeliveryService } from "../../../service/delivery.service";
import { MessagingService } from "../../../service/messaging.service";
import { CourierService } from "../../../service/courier.service";


/** Flat node with expandable and level information */
export class DynamicFlatNode {
  /**
   * コンストラクタ
   *
   * @param item
   * @param level
   * @param expandable
   * @param isLoading
   */
  constructor(
    private delivery: DeliveryService | null,
    public item: Category,
    public level = 1,
    public expandable = false,
    public isLoading = false) {
  }

  showContentList() {
    console.info("ノードボタンをクリック ", this.item.Id);
    if (this.delivery != null) {
      this.delivery.updateContentList(this.item.Id);
    }
  }
}

/**
 * Database for dynamic data. When expanding a node in the tree, the data source will need to fetch
 * the descendants data from the database.
 */
export class DynamicDatabase {
  dataMap = new Map<string, string[]>([
    ['Fruits', ['Apple', 'Orange', 'Banana']],
    ['Vegetables', ['Tomato', 'Potato', 'Onion']],
    ['Apple', ['Fuji', 'Macintosh']],
    ['Onion', ['Yellow', 'White', 'Purple']]
  ]);

  rootLevelNodes: string[] = ['Fruits', 'Vegetables'];

  /** Initial data from database */
  initialData(): DynamicFlatNode[] {
    return this.rootLevelNodes.map(name => new DynamicFlatNode(
      null,
      {
        Id: 1,
        Name: name,
        HasLinkSubCategoryFlag: true,
        Labels: []
      },
      0,
      true));
  }

  //getChildren(node: Category): Category[] | undefined {
  //    return this.dataMap.get(node);
  //}

  isExpandable(node: string): boolean {
    return this.dataMap.has(node);
  }
}

/**
 * File database, it can build a tree structured Json object from string.
 * Each node in Json object represents a file or a directory. For a file, it has filename and type.
 * For a directory, it has filename and children (a list of files or directories).
 * The input will be a json object string, and the output is a list of `FileNode` with nested
 * structure.
 */
@Injectable()
export class DynamicDataSource {

  dataChange = new BehaviorSubject<DynamicFlatNode[]>([]);

  get data(): DynamicFlatNode[] { return this.dataChange.value; }
  set data(value: DynamicFlatNode[]) {
    this.treeControl.dataNodes = value;
    this.dataChange.next(value);
  }


  /**
   * コンストラクタ
   *
   * @param treeControl
   * @param database
   * @param messaging メッセージングサービス
   * @param delivery デリバリーサービス
   */
  constructor(
    private treeControl: FlatTreeControl<DynamicFlatNode>,
    private database: DynamicDatabase,
    private courier: CourierService,
    private delivery: DeliveryService
  ) {
    this.courier.invalidateProp$.subscribe(
      (response: IpcUpdatePropResponse) => {
        if (response == undefined) return;
        if (response.PropertyName != "CategoryTree") return;

        console.info("[DynamicDataSource][invalidateProp$] subscribe - IN", response);
        let categoryId_parent: number = +response.Hint;
        const node = this.getNode(categoryId_parent);
        const index = this.data.indexOf(node); // 子階層を追加する親階層ノードのFlatNodeList内の位置を取得する
        const cat: Category[] = JSON.parse(response.Value);

        if (cat.length == 0 || index < 0) { // If no children, or cannot find the node, no op
          return;
        }


        const nodes = cat.map(prop =>
          new DynamicFlatNode(delivery, prop, node.level + 1, prop.HasLinkSubCategoryFlag));
        this.data.splice(index + 1, 0, ...nodes);

        // notify the change
        this.dataChange.next(this.data);
        node.isLoading = false;
      }
    );
  }

  connect(collectionViewer: CollectionViewer): Observable<DynamicFlatNode[]> {
    this.treeControl.expansionModel.onChange!.subscribe(change => {
      if ((change as SelectionChange<DynamicFlatNode>).added ||
        (change as SelectionChange<DynamicFlatNode>).removed) {
        this.handleTreeControl(change as SelectionChange<DynamicFlatNode>);
      }
    });

    return merge(collectionViewer.viewChange, this.dataChange).pipe(map(() => this.data));
  }

  /** Handle expand/collapse behaviors */
  handleTreeControl(change: SelectionChange<DynamicFlatNode>) {
    if (change.added) {
      change.added.forEach(node => this.toggleNode(node, true));
    }
    if (change.removed) {
      change.removed.slice().reverse().forEach(node => this.toggleNode(node, false));
    }
  }

  /**
   * カテゴリIDからノードを取得する
   *
   * @param categoryId
   */
  getNode(categoryId: number): DynamicFlatNode {
    return this.data.find(prop => prop.item.Id == categoryId);
  }

  /**
   * Toggle the node, remove from display list
   */
  toggleNode(node: DynamicFlatNode, expand: boolean) {
    node.isLoading = true;

    if (expand) {
      if (node.item.Id != 1 && node.item.Id != 2) {
        // DEBUG: デバッグコード
        console.info("デバッグモード中につき、カテゴリIDが所定の値以外では処理を行わない");
        return;
      }

      this.delivery.updateCategoryTree(node.item.Id); // カテゴリツリー取得呼び出し
    } else {
      setTimeout(() => {
        const index = this.data.indexOf(node); // 子階層を追加する親階層ノードのFlatNodeList内の位置を取得する
        let count = 0;
        for (let i = index + 1; i < this.data.length
          && this.data[i].level > node.level; i++ , count++) { }
        this.data.splice(index + 1, count);

        // notify the change
        this.dataChange.next(this.data);
        node.isLoading = false;
      }, 500);
    }
  }
}

/**
 * カテゴリツリーフラグメント
 */
@Component({
  selector: 'category-tree',
  templateUrl: './category-tree.fragment.html',
  styleUrls: ['./category-tree.fragment.scss'],
  providers: [DynamicDatabase]
})
export class CategoryTreeFragment {
  treeControl: FlatTreeControl<DynamicFlatNode>;

  dataSource: DynamicDataSource;

  getLevel = (node: DynamicFlatNode) => node.level;

  isExpandable = (node: DynamicFlatNode) => node.expandable;

  hasChild = (_: number, _nodeData: DynamicFlatNode) => _nodeData.expandable;

  /**
   * コンストラクタ
   *
   * @param database
   * @param messaging
   * @param delivery
   */
  constructor(
    private database: DynamicDatabase,
    private courier: CourierService,
    private delivery: DeliveryService,
  ) {
    this.treeControl = new FlatTreeControl<DynamicFlatNode>(this.getLevel, this.isExpandable);
    this.dataSource = new DynamicDataSource(this.treeControl, database, courier, delivery);

    this.dataSource.data = database.initialData();
  }
}
