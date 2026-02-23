const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Invoice = sequelize.define('Invoice', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    invoiceNumber: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    type: {
      type: DataTypes.ENUM('FeesInvoice', 'FeesReturn', 'PurchaseInvoice', 'PurchaseReturn'),
      allowNull: false
    },
    invoiceDate: {
      type: DataTypes.DATEONLY,
      defaultValue: DataTypes.NOW
    },
    studentId: DataTypes.INTEGER,
    supplierId: DataTypes.INTEGER,
    subTotal: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    taxAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    discountAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    totalAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    paidAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    paymentStatus: {
      type: DataTypes.ENUM('Pending', 'PartiallyPaid', 'Paid', 'Cancelled'),
      defaultValue: 'Pending'
    },
    paymentMethod: DataTypes.ENUM('Cash', 'Card', 'BankTransfer', 'Check', 'Online'),
    notes: DataTypes.TEXT,
    originalInvoiceId: DataTypes.INTEGER,
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'invoices',
    timestamps: true,
    paranoid: true,
    getterMethods: {
      dueAmount() {
        return parseFloat(this.totalAmount) - parseFloat(this.paidAmount);
      }
    }
  });

  return Invoice;
};
