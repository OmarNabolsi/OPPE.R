myData<-read.csv("C:/Users/on09/Desktop/Cogito/OPPE/Testing In R/Levene test GRP27.csv", sep=",")

install.packages("car")
library(car)

leveneTest(myData$Value, myData$Name, center=mean)

binVar<-myData$Name
scaleVar<-myData$Value

leveneTest(scaleVar, binVar, center=mean)$Pr[1]

t.test(scaleVar~binVar)$p.value
t.test(scaleVar~binVar, var.equal=TRUE)$p.value


install.packages("DBI")

library(DBI)
#con <- odbcConnect(Odbc::odbc(), Driver = "SQL Server", Server = "CNS02MABDSK01D\\SQL2017", Database = "OPPE_P_VALUE_AUTO", Trusted_Connection = "True")

con <- odbcConnect("DemoData")
SqlData <- sqlQuery(con, "select 
	CASE PayrollID 
		when 'BU52177' Then PayrollID
		else 'Peers' end as PayrollID
	,NumeratorValue
	From IndicatorsData
	where IndicatorsData.OppeIndicatorID = 12
		and IndicatorsData.OppePhysicianSubGroupID = 27")

nameVar<-SqlData$PayrollID
valueVar<-SqlData$NumeratorValue

test<- data.frame(nameVar,valueVar)

leveneTest(valueVar, nameVar, center = mean)$Pr[1]
t.test(valueVar~nameVar)#$p.value
t.test(valueVar~nameVar,var.equal=TRUE)$p.value
