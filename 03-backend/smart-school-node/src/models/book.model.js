const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Book = sequelize.define('Book', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    title: {
      type: DataTypes.STRING(500),
      allowNull: false
    },
    isbn: DataTypes.STRING(50),
    author: DataTypes.STRING(200),
    publisher: DataTypes.STRING(200),
    publishYear: DataTypes.INTEGER,
    edition: DataTypes.STRING(50),
    categoryId: DataTypes.INTEGER,
    subject: DataTypes.STRING(200),
    rackNumber: DataTypes.STRING(50),
    quantity: {
      type: DataTypes.INTEGER,
      defaultValue: 1
    },
    availableQuantity: {
      type: DataTypes.INTEGER,
      defaultValue: 1
    },
    price: DataTypes.DECIMAL(18, 2),
    description: DataTypes.TEXT,
    coverImage: DataTypes.STRING(500),
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'books',
    timestamps: true,
    paranoid: true
  });

  return Book;
};
