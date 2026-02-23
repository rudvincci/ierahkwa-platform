const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const BookCategory = sequelize.define('BookCategory', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    name: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    description: DataTypes.TEXT,
    parentId: DataTypes.INTEGER,
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'book_categories',
    timestamps: true,
    paranoid: true
  });

  return BookCategory;
};
