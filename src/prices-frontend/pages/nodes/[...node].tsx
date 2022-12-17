import React from "react";
import { NextPage } from "next";
import { useRouter } from "next/router";
import { PricingNode, PricingNodeByRtoAndNameQueryVariables, Query, Rtos } from "../../gql/codegen/graphql";
import ErrorPage from "next/error";
import { useQuery } from "@apollo/client";
import onCurrentPriceChangeSubscription from "../../gql/documents/subscriptions/onCurrentPriceChangeSubscription";
import { NodeHeader } from "../../components/node/node-header.component";
import { NodeChartContainer } from "../../components/node/node-chart-container.component";
import pricingNodeByRtoNameQuery from "../../gql/documents/queries/pricingNodeByRtoNameQuery";
import { Spinner } from "../../components/shared/spinner.component";
import { Metadata } from "../../components/core/metadata.component";

const NodePage: NextPage = () => {
    const router = useRouter();

    const rtoParam = router.query?.node ? router.query.node[0] : null;
    const nodeName = router.query?.node ? router.query.node[1] : null;

    let rto: Rtos | null = null;
    if (typeof rtoParam === "string") {
        const key = rtoParam.replace(/\w+/g, (w) => w[0].toUpperCase() + w.slice(1).toLowerCase());
        rto = Rtos[key as keyof typeof Rtos];
    }

    const { loading, data, subscribeToMore } = useQuery<Query, PricingNodeByRtoAndNameQueryVariables>(pricingNodeByRtoNameQuery, {
        variables: {
            name: nodeName ?? "",
            rto: rto ?? Rtos.Caiso,
        },
    });

    const subscribeToPriceChanges = () =>
        subscribeToMore({
            document: onCurrentPriceChangeSubscription,
        });

    let pricingNode: PricingNode | null = null;
    if (data) {
        pricingNode = data.pricingNodeByRtoAndName as PricingNode | null;
    }

    const renderContent = () => {
        if (!loading && !rto) {
            return <ErrorPage statusCode={404} title={`RTO "${rtoParam}" could not be found`} />;
        }

        if (!loading && !pricingNode) {
            return <ErrorPage statusCode={404} title={`Pricing Node "${nodeName}" could not be found`} />;
        }

        return (
            <>
                <Metadata title={loading ? "LMP Prices" : `LMP Prices: ${nodeName}`} description={loading ? "LMP Prices" : `LMP Prices: ${nodeName}`} />
                {loading ? (
                    <>
                        <Spinner />
                    </>
                ) : (
                    <>
                        <NodeHeader pricingNode={pricingNode!} />
                        <NodeChartContainer pricingNode={pricingNode!} subscribeToPriceChanges={subscribeToPriceChanges} />
                    </>
                )}
            </>
        );
    };

    return renderContent();
};

export default NodePage;
