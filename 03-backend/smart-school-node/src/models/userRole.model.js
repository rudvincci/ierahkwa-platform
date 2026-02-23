const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const UserRole = sequelize.define('UserRole', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    userId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    roleId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'user_roles',
    timestamps: true,
    indexes: [
      { unique: true, fields: ['userId', 'roleId'] }
    ]
  });

  return UserRole;
};
