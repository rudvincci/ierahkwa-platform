const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Product = sequelize.define('Product', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    name: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    code: DataTypes.STRING(50),
    barcode: DataTypes.STRING(50),
    description: DataTypes.TEXT,
    categoryId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    unitId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    purchasePrice: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    salePrice: {
      type: DataTypes.DECIMAL(18, 2),
      defaultValue: 0
    },
    quantity: {
      type: DataTypes.DECIMAL(18, 3),
      defaultValue: 0
    },
    minQuantity: {
      type: DataTypes.DECIMAL(18, 3),
      defaultValue: 0
    },
    image: DataTypes.STRING(500),
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'products',
    timestamps: true,
    paranoid: true
  });

  return Product;
};
