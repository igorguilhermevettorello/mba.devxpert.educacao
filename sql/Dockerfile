FROM mcr.microsoft.com/mssql/rhel/server:2019-CU1-rhel-8

USER root

RUN mkdir -p /app/work

WORKDIR /app/work

COPY . /app/work

EXPOSE 1433

CMD /bin/bash ./entrypoint.sh