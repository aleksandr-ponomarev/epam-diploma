//SECURITY GROUP
resource "aws_security_group" "secgrp-rds" {
  vpc_id      = module.vpc.vpc_id
  name        = "secgrp-rds"
  description = "Allow Postgres Port"
 
  ingress {
    description = "Allowing Connection for SSH"
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = module.vpc.private_subnets_cidr_blocks
  }
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "RDS"
  }
}

resource "aws_db_subnet_group" "rds-group" {
  name       = "rds-group"
  subnet_ids = module.vpc.private_subnets

  tags = {
    Name = "RDS group"
  }
}

//RDS INSTANCES
resource "aws_db_instance" "rds-dev" {
  engine               = "postgres"
  engine_version       = "12.5"
  instance_class       = "db.t2.micro"
  allocated_storage    = 10
  storage_type         = "gp2"
  name                 = "mydbdev"
  username             = "postgres"
  password             = "postgres"
  publicly_accessible = false
  skip_final_snapshot = true
  db_subnet_group_name = aws_db_subnet_group.rds-group.id
  vpc_security_group_ids = [aws_security_group.secgrp-rds.id]
  tags = {
  name = "RDS-DEV"
   }
}

resource "aws_db_instance" "rds-prod" {
  db_subnet_group_name = "main"
  engine               = "postgres"
  engine_version       = "12.5"
  instance_class       = "db.t2.micro"
  allocated_storage    = 10
  storage_type         = "gp2"
  name                 = "mydbprod"
  username             = "postgres"
  password             = "postgres"
  publicly_accessible = false
  skip_final_snapshot = true
  vpc_security_group_ids = [aws_security_group.secgrp-rds.id]
  tags = {
  name = "RDS-PROD"
   }
}