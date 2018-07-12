import {Component, Injectable} from '@angular/core';
import {CollectionViewer, SelectionChange} from '@angular/cdk/collections';
import {FlatTreeControl} from '@angular/cdk/tree';
import {BehaviorSubject, merge, Observable} from 'rxjs';
import { map } from 'rxjs/operators';
import { MessagingService } from './service/messaging.service';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { IpcUpdatePropResponse } from './service/contract/response.contract';
import { DeliveryService } from './service/delivery.service';

/** Flat node with expandable and level information */
export class DynamicFlatNode {
  constructor(public item: string, public level = 1, public expandable = false,
              public isLoading = false) {}
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
    return this.rootLevelNodes.map(name => new DynamicFlatNode(name, 0, true));
  }

  getChildren(node: string): string[] | undefined {
    return this.dataMap.get(node);
  }

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

  constructor(private treeControl: FlatTreeControl<DynamicFlatNode>,
              private database: DynamicDatabase) {}

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
   * Toggle the node, remove from display list
   */
  toggleNode(node: DynamicFlatNode, expand: boolean) {
    // node: 子階層を追加したい、親ノードのアイテム

    const children = this.database.getChildren(node.item);
    const index = this.data.indexOf(node); // 子階層を追加する親階層ノードのFlatNodeList内の位置を取得する

    if (!children || index < 0) { // If no children, or cannot find the node, no op
      return;
    }

    // ここで、nodeの子階層を取得する処理を実行する
    // 例:
    // setTimeoutを使って取得処理の遅延を演出している
    //
    // 仕組み案
    // ・コールバックを登録する

    node.isLoading = true;

    setTimeout(() => {
      if (expand) {
        const nodes = children.map(name =>
          new DynamicFlatNode(name, node.level + 1, this.database.isExpandable(name)));
        console.info("Node=", nodes);

        this.data.splice(index + 1, 0, ...nodes);
      } else {
        let count = 0;
        for (let i = index + 1; i < this.data.length
          && this.data[i].level > node.level; i++, count++) {}
        this.data.splice(index + 1, count);
      }

      // notify the change
      this.dataChange.next(this.data);
      node.isLoading = false;
    }, 1000);
  }
}


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [DynamicDatabase]
})
export class AppComponent {
  title = 'Asp Net Core 2.1 Angular 6 Template';

  constructor(database: DynamicDatabase,
    public messaging: MessagingService,
    private delivery: DeliveryService
  ) {
    this.treeControl = new FlatTreeControl<DynamicFlatNode>(this.getLevel, this.isExpandable);
    this.dataSource = new DynamicDataSource(this.treeControl, database);

    this.dataSource.data = database.initialData();
  }

  treeControl: FlatTreeControl<DynamicFlatNode>;

  dataSource: DynamicDataSource;

  getLevel = (node: DynamicFlatNode) => node.level;

  isExpandable = (node: DynamicFlatNode) => node.expandable;

  hasChild = (_: number, _nodeData: DynamicFlatNode) => _nodeData.expandable;

  // サンプル
  // ボタン押下で、InvalidatePropイベントを発生させる
  onDebugBasicButton() {
    console.info("onDebugBasicButton");
    let obj: IpcUpdatePropResponse = {
      PropertyName: "TEST Propety Name",
      Value: "VALUE"
    };
    this.messaging.fireInvalidateProp(obj);
  }

  /**
   * サンプル
   */
  onTRNS_TOPSCREEN() {
    console.info("onTRNS_TOPSCREEN");

    this.delivery.transTopScreen();
  }

  /**
   * サンプル
   */
  onDebugBasicButton2() {
    console.info("onDebugBasicButton2");

    this.delivery.executeDebugCommand("Nanikaなにか");
  }
}
