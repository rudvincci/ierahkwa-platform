const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const InvoicePayment = sequelize.define('InvoicePayment', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    invoiceId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    amount: {
      type: DataTypes.DECIMAL(18, 2),
      allowNull: false
    },
    paymentMethod: {
      type: DataTypes.ENUM('Cash', 'Card', 'BankTransfer', 'Check', 'Online'),
      allowNull: false
    },
    paymentDate: {
      type: DataTypes.DATEONLY,
      defaultValue: DataTypes.NOW
    },
    reference: DataTypes.STRING(100),
    notes: DataTypes.TEXT,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'invoice_payments',
    timestamps: true
  });

  return InvoicePayment;
};
