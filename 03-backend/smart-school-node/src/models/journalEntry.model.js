const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const JournalEntry = sequelize.define('JournalEntry', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    journalId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    accountId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    description: DataTypes.TEXT,
    debitAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    creditAmount: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    costCenterId: DataTypes.INTEGER,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'journal_entries',
    timestamps: true
  });

  return JournalEntry;
};
