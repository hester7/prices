import { Divider, Stack } from "@mui/material";
import { PricingNode, Query, Rtos } from "../../gql/codegen/graphql";
import { NodeTicker } from "./node-ticker.component";
import { useQuery } from "@apollo/client";
import pricingNodesQuery from "../../gql/documents/queries/pricingNodesQuery";
import onCurrentPriceChangeSubscription from "../../gql/documents/subscriptions/onCurrentPriceChangeSubscription";
import _ from "lodash";
import { Spinner } from "../shared/spinner.component";
import { Rto } from "./rto.component";
import { RtoNodeTypePricingNodes } from "../../types/RtoNodeTypePricingNodes";

type HomeProps = {};

export const Home = (props: HomeProps) => {
    const { loading, data, subscribeToMore } = useQuery<Query>(pricingNodesQuery);

    let pricingNodes: Array<PricingNode> = [];
    let rtoNodeTypes: Array<RtoNodeTypePricingNodes> = [];
    if (data) {
        pricingNodes = _.orderBy(data.pricingNodes, ["regionalTransmissionOperatorId", "pricingNodeTypeId", "name"], ["asc", "desc", "asc"]);

        rtoNodeTypes = Object.entries(
            _.groupBy(pricingNodes, (p) => {
                return [p.regionalTransmissionOperatorId, p.pricingNodeTypeId];
            })
        ).reduce((acc: any, [k, v]: [string, any[]]) => {
            var keys = _.split(k, ",");
            acc.push({
                rto: keys[0],
                pricingNodeTypeId: keys[1],
                pricingNodes: v,
            });
            return acc;
        }, []);
    }

    const subscribeToPriceChanges = () =>
        subscribeToMore({
            document: onCurrentPriceChangeSubscription,
        });

    return loading ? (
        <Spinner />
    ) : (
        <Stack gap={2}>
            <NodeTicker pricingNodes={pricingNodes} subscribeToPriceChanges={subscribeToPriceChanges} />
            <Divider />
            <Stack gap={4}>
                {rtoNodeTypes.map((x) => (
                    <Rto
                        key={`${x.rto}|${x.pricingNodeTypeId}`}
                        rto={x.rto}
                        pricingNodeTypeId={x.pricingNodeTypeId}
                        pricingNodes={x.pricingNodes}
                        subscribeToPriceChanges={subscribeToPriceChanges}
                    />
                ))}
            </Stack>
        </Stack>
    );
};
