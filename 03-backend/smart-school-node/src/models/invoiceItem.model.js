const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const InvoiceItem = sequelize.define('InvoiceItem', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    invoiceId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    productId: DataTypes.INTEGER,
    feesId: DataTypes.INTEGER,
    description: DataTypes.STRING(500),
    quantity: {
      type: DataTypes.DECIMAL(18, 3),
      defaultValue: 1
    },
    unitPrice: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    discountPercent: {
      type: DataTypes.DECIMAL(5, 2),
      defaultValue: 0
    },
    discountAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    taxPercent: {
      type: DataTypes.DECIMAL(5, 2),
      defaultValue: 0
    },
    taxAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    totalAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'invoice_items',
    timestamps: true
  });

  return InvoiceItem;
};
