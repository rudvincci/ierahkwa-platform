using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace InventoryManager.Forms
{
    /// <summary>
    /// Help documentation form with topics
    /// </summary>
    public class HelpForm : XtraForm
    {
        private TreeList _treeTopics = null!;
        private MemoEdit _txtContent = null!;

        private readonly Dictionary<string, string> _helpContent = new()
        {
            ["Getting Started"] = @"INVENTORY MANAGER PRO - GETTING STARTED
=========================================

Welcome to Inventory Manager Pro! This guide will help you get started with the application.

FIRST STEPS:
1. Login with your credentials (default: admin/admin123)
2. Navigate using the ribbon menu at the top
3. Use the navigation pane on the left for quick access

DASHBOARD:
- View overall statistics
- Monitor low stock items
- Track recent movements

KEYBOARD SHORTCUTS:
- F3: Quick Search
- F5: Refresh current view
- Ctrl+P: Print
",
            ["Products"] = @"PRODUCT MANAGEMENT
==================

ADDING PRODUCTS:
1. Go to Inventory > Products > New Product
2. Fill in required fields (Code, Name, Prices)
3. Set stock levels and category
4. Click Save

SEARCHING PRODUCTS:
- Use the search bar in the toolbar
- Press F3 for quick search
- Search by code, barcode, or name

EDITING PRODUCTS:
- Double-click a product in the grid
- Or select and click Edit button
- Make changes and save

STOCK STATUS:
- Green: Normal stock level
- Yellow: Near minimum stock
- Red: Below minimum stock
",
            ["Stock Movements"] = @"STOCK MOVEMENTS
===============

TYPES OF MOVEMENTS:
1. Purchase (Stock In) - Adding inventory from suppliers
2. Sale (Stock Out) - Removing inventory for sales
3. Adjustment - Correcting stock counts
4. Return - Customer returns
5. Damage - Damaged goods removal

RECORDING A MOVEMENT:
1. Go to Inventory > Stock In or Stock Out
2. Search and select product
3. Enter quantity and price
4. Add to list
5. Save all items

QUICK STOCK ENTRY:
- Use barcode scanner for fast entry
- Press Enter after each scan
- Items are automatically added
",
            ["Categories"] = @"CATEGORY MANAGEMENT
===================

Categories help organize your products for easier management and reporting.

CREATING CATEGORIES:
1. Go to Inventory > Categories
2. Click New Category
3. Enter code, name, and description
4. Select parent category if needed
5. Save

CATEGORY HIERARCHY:
- Categories can have parent categories
- Products inherit parent category properties
- Reports can be grouped by category
",
            ["Suppliers"] = @"SUPPLIER MANAGEMENT
===================

Manage your suppliers and their contact information.

ADDING SUPPLIERS:
1. Go to Inventory > Suppliers
2. Click New Supplier
3. Fill in contact details
4. Set payment terms
5. Save

SUPPLIER REPORTS:
- View products by supplier
- Track purchase history
- Export supplier lists
",
            ["Reports"] = @"REPORTS & EXPORTS
=================

AVAILABLE REPORTS:
1. Stock Report - Current inventory levels
2. Low Stock Alert - Items below minimum
3. Movement Report - Stock movement history
4. Statistics - Charts and analytics

EXPORT OPTIONS:
- Excel (.xlsx) - Full data export
- PDF - Formatted for printing
- HTML - Web viewable format
- Text (.txt) - Plain text export

PRINTING:
- Preview before printing
- Configure page settings
- Print directly from any grid
",
            ["Backup & Restore"] = @"DATABASE BACKUP & RESTORE
=========================

BACKUP:
1. Go to Tools > Backup Database
2. Choose save location
3. Enter filename
4. Click Save

RESTORE:
1. Go to Tools > Restore Database
2. Select backup file
3. Confirm restore
4. Application will restart

BEST PRACTICES:
- Backup regularly (daily recommended)
- Keep multiple backup copies
- Store backups in safe location
- Test restore periodically
",
            ["Multi-User Access"] = @"MULTI-USER ACCESS
=================

The application supports multiple users accessing the same database simultaneously.

FEATURES:
- WAL (Write-Ahead Logging) for concurrent access
- User session tracking
- Activity logging per user
- Role-based permissions

USER ROLES:
1. Admin - Full access to all features
2. Manager - Most features except settings
3. User - Basic inventory operations
4. ReadOnly - View only access

BEST PRACTICES:
- Each user should have their own account
- Use appropriate roles for security
- Monitor activity log regularly
",
            ["Troubleshooting"] = @"TROUBLESHOOTING
===============

COMMON ISSUES:

Login Problems:
- Check username and password
- Ensure account is active
- Contact administrator if locked

Database Errors:
- Check database file exists
- Verify file permissions
- Restart application

Slow Performance:
- Clear old activity logs
- Optimize database
- Check network connection (if shared)

Contact Support:
- Email: support@inventory.local
- Documentation: Help menu
"
        };

        public HelpForm()
        {
            InitializeComponents();
            LoadTopics();
        }

        private void InitializeComponents()
        {
            this.Text = "Help Documentation";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;

            // Splitter
            var splitter = new SplitContainerControl
            {
                Dock = DockStyle.Fill,
                SplitterPosition = 250,
                PanelVisibility = SplitPanelVisibility.Both
            };
            this.Controls.Add(splitter);

            // Left panel - Topics tree
            _treeTopics = new TreeList
            {
                Dock = DockStyle.Fill
            };
            _treeTopics.Columns.Add(new TreeListColumn { Caption = "Topics", VisibleIndex = 0 });
            _treeTopics.OptionsView.ShowColumns = false;
            _treeTopics.OptionsView.ShowIndicator = false;
            _treeTopics.FocusedNodeChanged += TreeTopics_FocusedNodeChanged;
            splitter.Panel1.Controls.Add(_treeTopics);

            // Right panel - Content
            _txtContent = new MemoEdit
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 11),
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            _txtContent.Properties.ReadOnly = true;
            _txtContent.Properties.WordWrap = true;
            splitter.Panel2.Controls.Add(_txtContent);
        }

        private void LoadTopics()
        {
            _treeTopics.BeginUnboundLoad();

            foreach (var topic in _helpContent.Keys)
            {
                _treeTopics.AppendNode(new object[] { topic }, null);
            }

            _treeTopics.EndUnboundLoad();

            // Select first topic
            if (_treeTopics.Nodes.Count > 0)
            {
                _treeTopics.FocusedNode = _treeTopics.Nodes[0];
            }
        }

        private void TreeTopics_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node != null)
            {
                var topic = e.Node.GetDisplayText(0);
                if (_helpContent.TryGetValue(topic, out var content))
                {
                    _txtContent.Text = content;
                }
            }
        }
    }
}
