const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const PostalReceive = sequelize.define('PostalReceive', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    referenceNo: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    fromTitle: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    fromAddress: DataTypes.TEXT,
    toTitle: DataTypes.STRING(200),
    receiveDate: {
      type: DataTypes.DATEONLY,
      defaultValue: DataTypes.NOW
    },
    type: DataTypes.STRING(50),
    notes: DataTypes.TEXT,
    attachment: DataTypes.STRING(500),
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'postal_receives',
    timestamps: true
  });

  return PostalReceive;
};
