FROM alpine:3.12.4

RUN addgroup -S ory; \
    adduser -S ory -G ory -D -H -s /bin/nologin
RUN apk add -U --no-cache ca-certificates

RUN wget https://github.com/ory/hydra/releases/download/v1.9.2/hydra_1.9.2_linux_arm32v7.tar.gz

RUN tar -xzf hydra_1.9.2_linux_arm32v7.tar.gz && rm hydra_1.9.2_linux_arm32v7.tar.gz

RUN cp hydra /usr/bin/hydra

# set up nsswitch.conf for Go's "netgo" implementation
# - https://github.com/golang/go/blob/go1.9.1/src/net/conf.go#L194-L275
RUN [ ! -e /etc/nsswitch.conf ] && echo 'hosts: files dns' > /etc/nsswitch.conf

USER ory

ENTRYPOINT ["hydra"]
CMD ["serve", "all"]

#CMD ["tail", "-f", "/dev/null"]