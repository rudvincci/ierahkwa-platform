const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const PostalDispatch = sequelize.define('PostalDispatch', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    referenceNo: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    toTitle: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    toAddress: DataTypes.TEXT,
    fromTitle: DataTypes.STRING(200),
    dispatchDate: {
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
    tableName: 'postal_dispatches',
    timestamps: true
  });

  return PostalDispatch;
};
