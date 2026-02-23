const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Parent = sequelize.define('Parent', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    userId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    firstName: {
      type: DataTypes.STRING(100),
      allowNull: false
    },
    lastName: {
      type: DataTypes.STRING(100),
      allowNull: false
    },
    email: DataTypes.STRING(200),
    phone: DataTypes.STRING(50),
    address: DataTypes.TEXT,
    occupation: DataTypes.STRING(100),
    relation: DataTypes.STRING(50),
    profileImage: DataTypes.STRING(500),
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'parents',
    timestamps: true,
    paranoid: true,
    getterMethods: {
      fullName() {
        return `${this.firstName} ${this.lastName}`.trim();
      }
    }
  });

  return Parent;
};
