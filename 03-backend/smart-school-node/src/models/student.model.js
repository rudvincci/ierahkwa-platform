const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Student = sequelize.define('Student', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    userId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    studentId: DataTypes.STRING(50),
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
    dateOfBirth: DataTypes.DATEONLY,
    gender: DataTypes.STRING(20),
    bloodGroup: DataTypes.STRING(10),
    profileImage: DataTypes.STRING(500),
    classRoomId: DataTypes.INTEGER,
    admissionDate: DataTypes.DATEONLY,
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'students',
    timestamps: true,
    paranoid: true,
    getterMethods: {
      fullName() {
        return `${this.firstName} ${this.lastName}`.trim();
      }
    }
  });

  return Student;
};
