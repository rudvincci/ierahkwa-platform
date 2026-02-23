const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const BorrowTransaction = sequelize.define('BorrowTransaction', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    transactionNumber: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    memberId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    bookId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    borrowDate: {
      type: DataTypes.DATEONLY,
      defaultValue: DataTypes.NOW
    },
    dueDate: {
      type: DataTypes.DATEONLY,
      allowNull: false
    },
    returnDate: DataTypes.DATEONLY,
    status: {
      type: DataTypes.ENUM('Borrowed', 'Returned', 'Overdue', 'Lost', 'Renewed'),
      defaultValue: 'Borrowed'
    },
    fineAmount: DataTypes.DECIMAL(18, 2),
    finePaid: {
      type: DataTypes.BOOLEAN,
      defaultValue: false
    },
    notes: DataTypes.TEXT,
    issuedBy: DataTypes.INTEGER,
    returnedTo: DataTypes.INTEGER,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'borrow_transactions',
    timestamps: true
  });

  return BorrowTransaction;
};
