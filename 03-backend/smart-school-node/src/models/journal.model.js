const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Journal = sequelize.define('Journal', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    journalNumber: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    journalDate: {
      type: DataTypes.DATEONLY,
      defaultValue: DataTypes.NOW
    },
    description: DataTypes.TEXT,
    reference: DataTypes.STRING(100),
    totalDebit: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    totalCredit: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    status: {
      type: DataTypes.ENUM('Draft', 'Pending', 'Approved', 'Posted', 'Cancelled'),
      defaultValue: 'Draft'
    },
    costCenterId: DataTypes.INTEGER,
    isPosted: {
      type: DataTypes.BOOLEAN,
      defaultValue: false
    },
    postedAt: DataTypes.DATE,
    postedBy: DataTypes.INTEGER,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'journals',
    timestamps: true,
    paranoid: true
  });

  return Journal;
};
